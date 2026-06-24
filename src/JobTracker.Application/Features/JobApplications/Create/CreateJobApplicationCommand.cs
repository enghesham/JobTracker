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
    ICompanyStore companyStore,
    IJobApplicationStore jobApplicationStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<CreateJobApplicationCommand, JobApplicationDto>
{
    public async Task<JobApplicationDto> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var company = await companyStore.GetForUserAsync(request.CompanyId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Company was not found for the current user.");

        var jobApplication = new JobApplication(
            userId,
            request.CompanyId,
            request.JobTitle,
            request.SourceUrl,
            DateTime.UtcNow);

        jobApplication.UpdateDetails(
            request.JobDescription,
            request.Location,
            request.FollowUpOnUtc,
            request.Notes);

        jobApplicationStore.Add(jobApplication);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
