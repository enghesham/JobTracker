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
    : DbContext(options), IUnitOfWork
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
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.MarkAsCreated(now);
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkAsUpdated(now);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
