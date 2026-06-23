using JobTracker.Domain.JobApplications;

namespace JobTracker.Application.Features.JobApplications.Common;

public sealed record JobApplicationDto(
    Guid Id,
    Guid CompanyId,
    string? CompanyName,
    string JobTitle,
    string? Location,
    string? SourceUrl,
    JobApplicationStatus Status,
    DateTime AppliedAtUtc,
    DateTime? FollowUpOnUtc,
    string? Notes);
