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
                logger.LogError(exception, "Follow-up reminder processing failed.");
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
            logger.LogInformation("Follow-up reminder {ReminderId} marked as sent.", reminder.Id);
        }

        if (dueReminders.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            InfrastructureDiagnostics.ReminderSuccessCount.Add(
                dueReminders.Length,
                new KeyValuePair<string, object?>("result", "sent"));

            logger.LogInformation(
                "Processed {ReminderCount} follow-up reminders due before {NowUtc}.",
                dueReminders.Length,
                nowUtc);
        }
    }
}
