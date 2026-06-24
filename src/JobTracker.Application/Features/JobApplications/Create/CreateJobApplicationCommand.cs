using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Domain.JobApplications;
using MediatR;

namespace JobTracker.Application.Features.JobApplications.Create;

public sealed record CreateJobApplicationCommand(
    Guid CompanyId,
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
        var company = dbContext.Companies.FirstOrDefault(candidate => candidate.Id == request.CompanyId && candidate.UserId == userId)
            ?? throw new InvalidOperationException("Company was not found for the current user.");

        var jobApplication = new JobApplication(
            userId,
            request.CompanyId,
            request.JobTitle.Trim(),
            string.IsNullOrWhiteSpace(request.SourceUrl) ? null : request.SourceUrl.Trim(),
            DateTime.UtcNow);

        jobApplication.UpdateDetails(
            string.IsNullOrWhiteSpace(request.JobDescription) ? null : request.JobDescription.Trim(),
            string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
            request.FollowUpOnUtc,
            string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim());

        dbContext.Add(jobApplication);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new JobApplicationDto(
            jobApplication.Id,
            jobApplication.CompanyId,
            company.Name,
            jobApplication.JobTitle,
            jobApplication.Location,
            jobApplication.SourceUrl,
            jobApplication.Status,
            jobApplication.AppliedAtUtc,
            jobApplication.FollowUpOnUtc,
            jobApplication.Notes);
    }
}
