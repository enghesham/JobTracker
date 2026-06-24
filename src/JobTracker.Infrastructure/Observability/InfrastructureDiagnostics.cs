using System.Diagnostics.Metrics;

namespace JobTracker.Infrastructure.Observability;

public static class InfrastructureDiagnostics
{
    public const string MeterName = "JobTracker.Infrastructure";

    public static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> ReminderSuccessCount = Meter.CreateCounter<long>(
        "jobtracker.reminder.success.count",
        description: "Number of follow-up reminders processed successfully.");

    public static readonly Counter<long> ReminderFailureCount = Meter.CreateCounter<long>(
        "jobtracker.reminder.failure.count",
        description: "Number of follow-up reminder processing failures.");

    public static readonly Counter<long> DatabaseErrorCount = Meter.CreateCounter<long>(
        "jobtracker.database.error.count",
        description: "Number of database command failures.");
}
