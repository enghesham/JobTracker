using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.JobApplications.Common;
using MediatR;

namespace JobTracker.Application.Features.JobApplications.GetMine;

public sealed record GetMyJobApplicationsQuery : IRequest<IReadOnlyCollection<JobApplicationDto>>;

public sealed class GetMyJobApplicationsQueryHandler(
    IJobApplicationReadService jobApplicationReadService,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyJobApplicationsQuery, IReadOnlyCollection<JobApplicationDto>>
{
    public Task<IReadOnlyCollection<JobApplicationDto>> Handle(GetMyJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        return jobApplicationReadService.GetMineAsync(userId, cancellationToken);
    }
}
