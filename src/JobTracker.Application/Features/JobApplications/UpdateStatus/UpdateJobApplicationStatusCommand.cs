using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Domain.JobApplications;
using MediatR;

namespace JobTracker.Application.Features.JobApplications.UpdateStatus;

public sealed record UpdateJobApplicationStatusCommand(Guid JobApplicationId, JobApplicationStatus Status) : IRequest<JobApplicationDto>;

public sealed record UpdateJobApplicationStatusRequest(JobApplicationStatus Status);

public sealed class UpdateJobApplicationStatusCommandHandler(
    ICompanyStore companyStore,
    IJobApplicationStore jobApplicationStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateJobApplicationStatusCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(UpdateJobApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var jobApplication = await jobApplicationStore.GetForUserAsync(request.JobApplicationId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Job application was not found.");

        jobApplication.ChangeStatus(request.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var company = await companyStore.GetForUserAsync(jobApplication.CompanyId, userId, cancellationToken);

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
