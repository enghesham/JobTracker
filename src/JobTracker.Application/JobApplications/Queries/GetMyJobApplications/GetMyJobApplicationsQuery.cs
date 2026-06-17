using JobTracker.Application.Common.Interfaces;
using MediatR;

namespace JobTracker.Application.JobApplications.Queries.GetMyJobApplications;

public sealed record GetMyJobApplicationsQuery : IRequest<IReadOnlyCollection<JobApplicationDto>>;

public sealed class GetMyJobApplicationsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyJobApplicationsQuery, IReadOnlyCollection<JobApplicationDto>>
{
    public Task<IReadOnlyCollection<JobApplicationDto>> Handle(GetMyJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");

        IReadOnlyCollection<JobApplicationDto> applications = dbContext.JobApplications
            .Where(application => application.UserId == userId)
            .OrderByDescending(application => application.AppliedAtUtc)
            .Select(application => new JobApplicationDto(
                application.Id,
                application.CompanyId,
                application.Company == null ? null : application.Company.Name,
                application.JobTitle,
                application.Location,
                application.SourceUrl,
                application.Status,
                application.AppliedAtUtc,
                application.FollowUpOnUtc,
                application.Notes))
            .ToArray();

        return Task.FromResult(applications);
    }
}
