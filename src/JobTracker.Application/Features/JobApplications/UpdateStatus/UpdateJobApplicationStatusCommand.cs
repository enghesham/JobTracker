using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Domain.JobApplications;
using MediatR;

namespace JobTracker.Application.Features.JobApplications.UpdateStatus;

public sealed record UpdateJobApplicationStatusCommand(Guid JobApplicationId, JobApplicationStatus Status) : IRequest<JobApplicationDto>;

public sealed record UpdateJobApplicationStatusRequest(JobApplicationStatus Status);

public sealed class UpdateJobApplicationStatusCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateJobApplicationStatusCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(UpdateJobApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var jobApplication = dbContext.JobApplications
            .FirstOrDefault(application => application.Id == request.JobApplicationId && application.UserId == userId)
            ?? throw new InvalidOperationException("Job application was not found.");

        jobApplication.ChangeStatus(request.Status);
        await dbContext.SaveChangesAsync(cancellationToken);

        var company = dbContext.Companies.FirstOrDefault(candidate => candidate.Id == jobApplication.CompanyId);

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
