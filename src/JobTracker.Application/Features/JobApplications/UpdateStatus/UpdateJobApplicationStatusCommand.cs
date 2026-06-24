using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Common.Results;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Domain.JobApplications;
using MediatR;

namespace JobTracker.Application.Features.JobApplications.UpdateStatus;

public sealed record UpdateJobApplicationStatusCommand(Guid JobApplicationId, JobApplicationStatus Status) : IRequest<Result<JobApplicationDto>>;

public sealed record UpdateJobApplicationStatusRequest(JobApplicationStatus Status);

public sealed class UpdateJobApplicationStatusCommandHandler(
    ICompanyStore companyStore,
    IJobApplicationStore jobApplicationStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateJobApplicationStatusCommand, Result<JobApplicationDto>>
{
    public async Task<Result<JobApplicationDto>> Handle(UpdateJobApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<JobApplicationDto>.Failure(Error.Unauthorized(
                "user-not-authenticated",
                "User is not authenticated",
                "The current request does not contain an authenticated user."));
        }

        var jobApplication = await jobApplicationStore.GetForUserAsync(request.JobApplicationId, userId.Value, cancellationToken);
        if (jobApplication is null)
        {
            return Result<JobApplicationDto>.Failure(Error.NotFound(
                "job-application-not-found",
                "Job application not found",
                "The requested job application does not exist."));
        }

        jobApplication.ChangeStatus(request.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var company = await companyStore.GetForUserAsync(jobApplication.CompanyId, userId.Value, cancellationToken);

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

