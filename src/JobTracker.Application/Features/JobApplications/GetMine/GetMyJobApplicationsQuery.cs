using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Common.Results;
using JobTracker.Application.Features.JobApplications.Common;
using MediatR;

namespace JobTracker.Application.Features.JobApplications.GetMine;

public sealed record GetMyJobApplicationsQuery : IRequest<Result<IReadOnlyCollection<JobApplicationDto>>>;

public sealed class GetMyJobApplicationsQueryHandler(
    IJobApplicationReadService jobApplicationReadService,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyJobApplicationsQuery, Result<IReadOnlyCollection<JobApplicationDto>>>
{
    public async Task<Result<IReadOnlyCollection<JobApplicationDto>>> Handle(GetMyJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<IReadOnlyCollection<JobApplicationDto>>.Failure(Error.Unauthorized(
                "user-not-authenticated",
                "User is not authenticated",
                "The current request does not contain an authenticated user."));
        }

        var applications = await jobApplicationReadService.GetMineAsync(userId.Value, cancellationToken);
        return Result<IReadOnlyCollection<JobApplicationDto>>.Success(applications);
    }
}
