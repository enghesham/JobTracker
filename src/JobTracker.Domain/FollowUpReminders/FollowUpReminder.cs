using JobTracker.Domain.Common;
using JobTracker.Domain.JobApplications;

namespace JobTracker.Domain.FollowUpReminders;

public sealed class FollowUpReminder : BaseEntity
{
    public const int MessageMaxLength = 1000;

    private FollowUpReminder() { }

    public FollowUpReminder(Guid jobApplicationId, DateTime remindAtUtc, string? message)
    {
        DomainGuard.AgainstEmpty(jobApplicationId, nameof(jobApplicationId));

        if (remindAtUtc == default)
        {
            throw new DomainException("Reminder date is required.");
        }

        JobApplicationId = jobApplicationId;
        RemindAtUtc = remindAtUtc;
        Message = DomainGuard.Optional(message, nameof(message), MessageMaxLength);
    }

    public Guid JobApplicationId { get; private set; }
    public JobApplication? JobApplication { get; private set; }
    public DateTime RemindAtUtc { get; private set; }
    public bool IsSent { get; private set; }
    public string? Message { get; private set; }

    public void MarkAsSent()
    {
        if (IsSent)
        {
            return;
        }

        IsSent = true;
        MarkAsUpdated();
    }
}
