using JobTracker.Domain.Entities;
// using Microsoft.EntityFrameworkCore;

namespace JobTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<User> Users { get; }
    IQueryable<Company> Companies { get; }
    IQueryable<JobApplication> JobApplications { get; }
    IQueryable<FollowUpReminder> FollowUpReminders { get; }

    void Add<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
