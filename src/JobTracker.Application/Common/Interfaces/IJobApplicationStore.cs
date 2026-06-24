using JobTracker.Domain.JobApplications;

namespace JobTracker.Application.Common.Interfaces;

public interface IJobApplicationStore
{
    Task<JobApplication?> GetForUserAsync(Guid jobApplicationId, Guid userId, CancellationToken cancellationToken = default);
    void Add(JobApplication jobApplication);
}
