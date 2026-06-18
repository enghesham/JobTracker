using JobTracker.Domain.Common;
using JobTracker.Domain.Enums;

namespace JobTracker.Domain.Entities;

public sealed class JobApplication : BaseEntity
{
    private JobApplication() { }

    public JobApplication(
        Guid userId,
        Guid companyId,
        string jobTitle,
        string? sourceUrl,
        DateTime appliedAtUtc)
    {
        UserId = userId;
        CompanyId = companyId;
        JobTitle = jobTitle;
        SourceUrl = sourceUrl;
        AppliedAtUtc = appliedAtUtc;
        Status = ApplicationStatus.Applied;
    }

    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string JobTitle { get; private set; } = default!;
    public string? JobDescription { get; private set; }
    public string? Location { get; private set; }
    public string? SourceUrl { get; private set; }
    public ApplicationStatus Status { get; private set; } = ApplicationStatus.Applied;
    public DateTime? FollowUpOnUtc { get; private set; }
    public DateTime AppliedAtUtc { get; private set; } = DateTime.UtcNow;
    public string? Notes { get; private set; }

    public User User { get; private set; } = default!;
    public Company Company { get; private set; } = default!;
    public ICollection<FollowUpReminder> FollowUpReminders { get; private set; } = new List<FollowUpReminder>();

    public void UpdateDetails(string? jobDescription, string? location, DateTime? followUpOnUtc, string? notes)
    {
        JobDescription = jobDescription;
        Location = location;
        FollowUpOnUtc = followUpOnUtc;
        Notes = notes;
        MarkAsUpdated();
    }

    public void ChangeStatus(ApplicationStatus status)
    {
        if (Status == ApplicationStatus.Rejected && status == ApplicationStatus.Offer)
        {
            throw new InvalidOperationException("Cannot move rejected application to offer.");
        }

        Status = status;
        MarkAsUpdated();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        MarkAsUpdated();
    }
}
