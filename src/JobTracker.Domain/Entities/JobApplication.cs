using JobTracker.Domain.Common;
using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities;

public sealed class JobApplication : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public string JobTitle { get; set; } = string.Empty;
    public string? JobDescription { get; set; }
    public string? Location { get; set; }
    public string? SourceUrl { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public DateTime AppliedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? FollowUpOnUtc { get; set; }
    public string? Notes { get; set; }

    public ICollection<FollowUpReminder> FollowUpReminders { get; set; } = new List<FollowUpReminder>();
}
