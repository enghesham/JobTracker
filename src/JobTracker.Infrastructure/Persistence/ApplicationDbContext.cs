using JobTracker.Application.Common.Interfaces;
using JobTracker.Domain.Common;
using JobTracker.Domain.Companies;
using JobTracker.Domain.FollowUpReminders;
using JobTracker.Domain.JobApplications;
using JobTracker.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobTracker.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<FollowUpReminder> FollowUpReminders => Set<FollowUpReminder>();

    IQueryable<User> IApplicationDbContext.Users => Users;
    IQueryable<Company> IApplicationDbContext.Companies => Companies;
    IQueryable<JobApplication> IApplicationDbContext.JobApplications => JobApplications;
    IQueryable<FollowUpReminder> IApplicationDbContext.FollowUpReminders => FollowUpReminders;

    void IApplicationDbContext.Add<TEntity>(TEntity entity) => Set<TEntity>().Add(entity);

    void IApplicationDbContext.Remove<TEntity>(TEntity entity) => Set<TEntity>().Remove(entity);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
