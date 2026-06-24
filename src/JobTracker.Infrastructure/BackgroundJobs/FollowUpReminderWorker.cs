using JobTracker.Infrastructure.Observability;
using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.BackgroundJobs;

public sealed class FollowUpReminderWorker(
    IServiceScopeFactory scopeFactory,
    TimeProvider timeProvider,
    ILogger<FollowUpReminderWorker> logger) : BackgroundService
{
    private static readonly Action<ILogger, Exception?> ReminderProcessingFailed =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2000, nameof(ReminderProcessingFailed)),
            "Follow-up reminder processing failed.");

    private static readonly Action<ILogger, Guid, Exception?> ReminderMarkedAsSent =
        LoggerMessage.Define<Guid>(
            LogLevel.Information,
            new EventId(2001, nameof(ReminderMarkedAsSent)),
            "Follow-up reminder {ReminderId} marked as sent.");

    private static readonly Action<ILogger, int, DateTimeOffset, Exception?> RemindersProcessed =
        LoggerMessage.Define<int, DateTimeOffset>(
            LogLevel.Information,
            new EventId(2002, nameof(RemindersProcessed)),
            "Processed {ReminderCount} follow-up reminders due before {NowUtc}.");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessDueReminders(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                InfrastructureDiagnostics.ReminderFailureCount.Add(1);
                ReminderProcessingFailed(logger, exception);
            }
        }
    }

    private async Task ProcessDueReminders(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var nowUtc = timeProvider.GetUtcNow();

        var dueReminders = await dbContext.FollowUpReminders
            .Where(reminder => !reminder.IsSent && reminder.RemindAtUtc <= nowUtc)
            .ToArrayAsync(cancellationToken);

        foreach (var reminder in dueReminders)
        {
            reminder.MarkAsSent();
            ReminderMarkedAsSent(logger, reminder.Id, null);
        }

        if (dueReminders.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            InfrastructureDiagnostics.ReminderSuccessCount.Add(
                dueReminders.Length,
                new KeyValuePair<string, object?>("result", "sent"));

            RemindersProcessed(logger, dueReminders.Length, nowUtc, null);
        }
    }
}
