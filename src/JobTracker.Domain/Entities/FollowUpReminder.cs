using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities;

public sealed class FollowUpReminder : BaseEntity
{
    private FollowUpReminder() { }

    public FollowUpReminder(Guid jobApplicationId, DateTime remindAtUtc, string? message)
    {
        JobApplicationId = jobApplicationId;
        RemindAtUtc = remindAtUtc;
        Message = message;
    }

    public Guid JobApplicationId { get; private set; }
    public JobApplication? JobApplication { get; private set; }
    public DateTime RemindAtUtc { get; private set; }
    public bool IsSent { get; private set; }
    public string? Message { get; private set; }

    public void MarkAsSent()
    {
        IsSent = true;
        MarkAsUpdated();
    }
}
