using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.ReadServices;

public sealed class JobApplicationReadService(ApplicationDbContext dbContext) : IJobApplicationReadService
{
    public async Task<IReadOnlyCollection<JobApplicationDto>> GetMineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobApplications
            .AsNoTracking()
            .Where(application => application.UserId == userId)
            .OrderByDescending(application => application.AppliedAtUtc)
            .Select(application => new JobApplicationDto(
                application.Id,
                application.CompanyId,
                application.Company.Name,
                application.JobTitle,
                application.Location,
                application.SourceUrl,
                application.Status,
                application.AppliedAtUtc,
                application.FollowUpOnUtc,
                application.Notes))
            .ToArrayAsync(cancellationToken);
    }
}
