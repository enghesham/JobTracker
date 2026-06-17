using JobTracker.Domain.Enums;

namespace JobTracker.Application.JobApplications;

public sealed record JobApplicationDto(
    Guid Id,
    Guid? CompanyId,
    string? CompanyName,
    string JobTitle,
    string? Location,
    string? SourceUrl,
    ApplicationStatus Status,
    DateTime AppliedAtUtc,
    DateTime? FollowUpOnUtc,
    string? Notes);
