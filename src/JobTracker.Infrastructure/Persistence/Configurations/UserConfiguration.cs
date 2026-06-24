using JobTracker.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.FullName)
            .HasMaxLength(User.FullNameMaxLength)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(User.EmailMaxLength)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(User.PasswordHashMaxLength)
            .IsRequired();

        builder.Navigation(user => user.Companies)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(user => user.JobApplications)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
