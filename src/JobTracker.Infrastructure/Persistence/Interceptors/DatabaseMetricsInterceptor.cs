using System.Data.Common;
using JobTracker.Infrastructure.Observability;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.Persistence.Interceptors;

public sealed class DatabaseMetricsInterceptor(ILogger<DatabaseMetricsInterceptor> logger) : DbCommandInterceptor
{
    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        RecordFailure(command, eventData);
        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        RecordFailure(command, eventData);
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private void RecordFailure(DbCommand command, CommandErrorEventData eventData)
    {
        var provider = eventData.Context?.Database.ProviderName ?? "unknown";

        InfrastructureDiagnostics.DatabaseErrorCount.Add(
            1,
            new KeyValuePair<string, object?>("provider", provider));

        logger.LogError(
            eventData.Exception,
            "Database command failed for provider {DatabaseProvider} after {ElapsedMilliseconds} ms.",
            provider,
            eventData.Duration.TotalMilliseconds);
    }
}
