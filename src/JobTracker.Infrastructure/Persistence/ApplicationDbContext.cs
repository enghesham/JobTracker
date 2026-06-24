using System.Reflection;
using JobTracker.Application.Common.Interfaces;
using JobTracker.Domain.Common;
using JobTracker.Domain.Companies;
using JobTracker.Domain.FollowUpReminders;
using JobTracker.Domain.JobApplications;
using JobTracker.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    TimeProvider timeProvider) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<FollowUpReminder> FollowUpReminders => Set<FollowUpReminder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = timeProvider.GetUtcNow();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.MarkAsCreated(nowUtc);
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkAsUpdated(nowUtc);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
