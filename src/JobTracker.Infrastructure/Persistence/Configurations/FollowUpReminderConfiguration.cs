using JobTracker.Domain.FollowUpReminders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Persistence.Configurations;

public sealed class FollowUpReminderConfiguration : IEntityTypeConfiguration<FollowUpReminder>
{
    public void Configure(EntityTypeBuilder<FollowUpReminder> builder)
    {
        builder.ToTable("FollowUpReminders");
        builder.HasKey(reminder => reminder.Id);

        builder.Property(reminder => reminder.Message)
            .HasMaxLength(FollowUpReminder.MessageMaxLength);
    }
}
