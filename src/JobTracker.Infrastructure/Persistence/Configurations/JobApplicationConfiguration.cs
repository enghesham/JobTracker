using JobTracker.Domain.JobApplications;
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
            .HasMaxLength(JobApplication.JobTitleMaxLength)
            .IsRequired();

        builder.Property(application => application.JobDescription)
            .HasMaxLength(JobApplication.JobDescriptionMaxLength);

        builder.Property(application => application.Location)
            .HasMaxLength(JobApplication.LocationMaxLength);

        builder.Property(application => application.SourceUrl)
            .HasMaxLength(JobApplication.SourceUrlMaxLength);

        builder.Property(application => application.Notes)
            .HasMaxLength(JobApplication.NotesMaxLength);

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

        builder.Navigation(application => application.FollowUpReminders)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
