using JobTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobTracker.Infrastructure.BackgroundJobs;

public sealed class FollowUpReminderWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<FollowUpReminderWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessDueReminders(stoppingToken);
        }
    }

    private async Task ProcessDueReminders(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var now = DateTime.UtcNow;

        var dueReminders = dbContext.FollowUpReminders
            .Where(reminder => !reminder.IsSent && reminder.RemindAtUtc <= now)
            .ToArray();

        foreach (var reminder in dueReminders)
        {
            reminder.MarkAsSent();
            logger.LogInformation("Follow-up reminder {ReminderId} marked as sent.", reminder.Id);
        }

        if (dueReminders.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

