using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.JobApplications;
using JobTracker.Domain.Entities;
using JobTracker.Domain.Enums;
using MediatR;

namespace JobTracker.Application.JobApplications.Commands.CreateJobApplication;

public sealed record CreateJobApplicationCommand(
    Guid? CompanyId,
    string JobTitle,
    string? JobDescription,
    string? Location,
    string? SourceUrl,
    DateTime? FollowUpOnUtc,
    string? Notes) : IRequest<JobApplicationDto>;

public sealed class CreateJobApplicationCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<CreateJobApplicationCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");

        Company? company = null;
        if (request.CompanyId.HasValue)
        {
            company = dbContext.Companies.FirstOrDefault(candidate => candidate.Id == request.CompanyId.Value)
                ?? throw new InvalidOperationException("Company was not found.");
        }

        var jobApplication = new JobApplication
        {
            UserId = userId,
            CompanyId = request.CompanyId,
            Company = company,
            JobTitle = request.JobTitle.Trim(),
            JobDescription = string.IsNullOrWhiteSpace(request.JobDescription) ? null : request.JobDescription.Trim(),
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
            SourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl) ? null : request.SourceUrl.Trim(),
            FollowUpOnUtc = request.FollowUpOnUtc,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Status = ApplicationStatus.Applied
        };

        dbContext.Add(jobApplication);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new JobApplicationDto(
            jobApplication.Id,
            jobApplication.CompanyId,
            company?.Name,
            jobApplication.JobTitle,
            jobApplication.Location,
            jobApplication.SourceUrl,
            jobApplication.Status,
            jobApplication.AppliedAtUtc,
            jobApplication.FollowUpOnUtc,
            jobApplication.Notes);
    }
}
