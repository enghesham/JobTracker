using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Persistence.Configurations;

public sealed class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");
        builder.HasKey(application => application.Id);

        builder.Property(application => application.JobTitle)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(application => application.JobDescription)
            .HasMaxLength(4000);

        builder.Property(application => application.Location)
            .HasMaxLength(200);

        builder.Property(application => application.SourceUrl)
            .HasMaxLength(1000);

        builder.Property(application => application.Notes)
            .HasMaxLength(4000);

        builder.Property(application => application.Status)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.HasOne(application => application.User)
            .WithMany(user => user.JobApplications)
            .HasForeignKey(application => application.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(application => application.Company)
            .WithMany(company => company.JobApplications)
            .HasForeignKey(application => application.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(application => application.FollowUpReminders)
            .WithOne(reminder => reminder.JobApplication)
            .HasForeignKey(reminder => reminder.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

