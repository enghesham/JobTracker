using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities;

public sealed class FollowUpReminder : BaseEntity
{
    public Guid JobApplicationId { get; set; }
    public JobApplication? JobApplication { get; set; }

    public DateTime RemindAtUtc { get; set; }
    public bool IsSent { get; set; }
    public string? Message { get; set; }
}
