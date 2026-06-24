using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Common.Results;
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
    DateTimeOffset? FollowUpOnUtc,
    string? Notes) : IRequest<Result<JobApplicationDto>>;

public sealed class CreateJobApplicationCommandHandler(
    ICompanyStore companyStore,
    IJobApplicationStore jobApplicationStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider) : IRequestHandler<CreateJobApplicationCommand, Result<JobApplicationDto>>
{
    public async Task<Result<JobApplicationDto>> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<JobApplicationDto>.Failure(Error.Unauthorized(
                "user-not-authenticated",
                "User is not authenticated",
                "The current request does not contain an authenticated user."));
        }

        var company = await companyStore.GetForUserAsync(request.CompanyId, userId.Value, cancellationToken);
        if (company is null)
        {
            return Result<JobApplicationDto>.Failure(Error.NotFound(
                "company-not-found",
                "Company not found",
                "The requested company does not exist for the current user."));
        }

        var nowUtc = timeProvider.GetUtcNow();

        var jobApplication = new JobApplication(
            userId.Value,
            request.CompanyId,
            request.JobTitle,
            request.SourceUrl,
            nowUtc,
            nowUtc);

        jobApplication.UpdateDetails(
            request.JobDescription,
            request.Location,
            request.FollowUpOnUtc,
            request.Notes,
            nowUtc);

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
