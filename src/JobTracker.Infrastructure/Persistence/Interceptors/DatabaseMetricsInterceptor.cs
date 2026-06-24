using System.Data.Common;
using JobTracker.Infrastructure.Observability;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.Persistence.Interceptors;

public sealed class DatabaseMetricsInterceptor(ILogger<DatabaseMetricsInterceptor> logger) : DbCommandInterceptor
{
    private static readonly Action<ILogger, string, double, Exception?> DatabaseCommandFailed =
        LoggerMessage.Define<string, double>(
            LogLevel.Error,
            new EventId(3000, nameof(DatabaseCommandFailed)),
            "Database command failed for provider {DatabaseProvider} after {ElapsedMilliseconds} ms.");

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        RecordFailure(eventData);
        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        RecordFailure(eventData);
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    private void RecordFailure(CommandErrorEventData eventData)
    {
        var provider = eventData.Context?.Database.ProviderName ?? "unknown";

        InfrastructureDiagnostics.DatabaseErrorCount.Add(
            1,
            new KeyValuePair<string, object?>("provider", provider));

        DatabaseCommandFailed(logger, provider, eventData.Duration.TotalMilliseconds, eventData.Exception);
    }
}
