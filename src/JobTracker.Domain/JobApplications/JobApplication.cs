using JobTracker.Domain.Common;
using JobTracker.Domain.Companies;
using JobTracker.Domain.FollowUpReminders;
using JobTracker.Domain.Users;

namespace JobTracker.Domain.JobApplications;

public sealed class JobApplication : BaseEntity
{
    public const int JobTitleMaxLength = 250;
    public const int JobDescriptionMaxLength = 4000;
    public const int LocationMaxLength = 200;
    public const int SourceUrlMaxLength = 1000;
    public const int NotesMaxLength = 4000;

    private static readonly IReadOnlyDictionary<JobApplicationStatus, JobApplicationStatus[]> AllowedTransitions =
        new Dictionary<JobApplicationStatus, JobApplicationStatus[]>
        {
            [JobApplicationStatus.Draft] = new[] { JobApplicationStatus.Applied, JobApplicationStatus.Withdrawn },
            [JobApplicationStatus.Applied] = new[] { JobApplicationStatus.Screening, JobApplicationStatus.Interview, JobApplicationStatus.Offer, JobApplicationStatus.Rejected, JobApplicationStatus.Withdrawn },
            [JobApplicationStatus.Screening] = new[] { JobApplicationStatus.Interview, JobApplicationStatus.Offer, JobApplicationStatus.Rejected, JobApplicationStatus.Withdrawn },
            [JobApplicationStatus.Interview] = new[] { JobApplicationStatus.Offer, JobApplicationStatus.Rejected, JobApplicationStatus.Withdrawn },
            [JobApplicationStatus.Offer] = new[] { JobApplicationStatus.Withdrawn },
            [JobApplicationStatus.Rejected] = Array.Empty<JobApplicationStatus>(),
            [JobApplicationStatus.Withdrawn] = Array.Empty<JobApplicationStatus>()
        };

    private readonly List<FollowUpReminder> _followUpReminders = new();

    private JobApplication() { }

    public JobApplication(
        Guid userId,
        Guid companyId,
        string jobTitle,
        string? sourceUrl,
        DateTimeOffset appliedAtUtc,
        DateTimeOffset nowUtc)
    {
        DomainGuard.AgainstEmpty(userId, nameof(userId));
        DomainGuard.AgainstEmpty(companyId, nameof(companyId));

        appliedAtUtc = appliedAtUtc.ToUniversalTime();
        nowUtc = nowUtc.ToUniversalTime();

        if (appliedAtUtc == default)
        {
            throw new DomainException("Applied date is required.");
        }

        if (appliedAtUtc > nowUtc.AddMinutes(5))
        {
            throw new DomainException("Applied date cannot be in the future.");
        }

        UserId = userId;
        CompanyId = companyId;
        JobTitle = DomainGuard.Required(jobTitle, nameof(jobTitle), JobTitleMaxLength);
        SourceUrl = DomainGuard.OptionalHttpUrl(sourceUrl, nameof(sourceUrl), SourceUrlMaxLength);
        AppliedAtUtc = appliedAtUtc;
        Status = JobApplicationStatus.Applied;
    }

    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string JobTitle { get; private set; } = default!;
    public string? JobDescription { get; private set; }
    public string? Location { get; private set; }
    public string? SourceUrl { get; private set; }
    public JobApplicationStatus Status { get; private set; } = JobApplicationStatus.Applied;
    public DateTimeOffset? FollowUpOnUtc { get; private set; }
    public DateTimeOffset AppliedAtUtc { get; private set; }
    public string? Notes { get; private set; }

    public User User { get; private set; } = default!;
    public Company Company { get; private set; } = default!;
    public IReadOnlyCollection<FollowUpReminder> FollowUpReminders => _followUpReminders;

    public void UpdateDetails(
        string? jobDescription,
        string? location,
        DateTimeOffset? followUpOnUtc,
        string? notes,
        DateTimeOffset nowUtc)
    {
        nowUtc = nowUtc.ToUniversalTime();
        followUpOnUtc = followUpOnUtc?.ToUniversalTime();

        if (followUpOnUtc.HasValue && followUpOnUtc.Value.UtcDateTime.Date < nowUtc.UtcDateTime.Date)
        {
            throw new DomainException("Follow-up date cannot be in the past.");
        }

        JobDescription = DomainGuard.Optional(jobDescription, nameof(jobDescription), JobDescriptionMaxLength);
        Location = DomainGuard.Optional(location, nameof(location), LocationMaxLength);
        FollowUpOnUtc = followUpOnUtc;
        Notes = DomainGuard.Optional(notes, nameof(notes), NotesMaxLength);
    }

    public void ChangeStatus(JobApplicationStatus status)
    {
        if (status == Status)
        {
            return;
        }

        if (!AllowedTransitions.TryGetValue(Status, out var allowedStatuses) || !allowedStatuses.Contains(status))
        {
            throw new DomainException($"Cannot move job application from {Status} to {status}.");
        }

        Status = status;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = DomainGuard.Optional(notes, nameof(notes), NotesMaxLength);
    }
}
