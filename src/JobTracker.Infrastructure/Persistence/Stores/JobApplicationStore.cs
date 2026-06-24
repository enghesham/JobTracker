using JobTracker.Application.Common.Interfaces;
using JobTracker.Domain.JobApplications;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.Stores;

public sealed class JobApplicationStore(ApplicationDbContext dbContext) : IJobApplicationStore
{
    public Task<JobApplication?> GetForUserAsync(Guid jobApplicationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.JobApplications.FirstOrDefaultAsync(
            application => application.Id == jobApplicationId && application.UserId == userId,
            cancellationToken);
    }

    public void Add(JobApplication jobApplication)
    {
        dbContext.JobApplications.Add(jobApplication);
    }
}
