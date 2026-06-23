namespace JobTracker.Application.Features.JobApplications.Common;

public interface IJobApplicationReadService
{
    Task<IReadOnlyCollection<JobApplicationDto>> GetMineAsync(Guid userId, CancellationToken cancellationToken = default);
}
