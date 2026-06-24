using JobTracker.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        builder.HasKey(company => company.Id);

        builder.Property(company => company.Name)
            .HasMaxLength(Company.NameMaxLength)
            .IsRequired();

        builder.Property(company => company.NormalizedName)
            .HasMaxLength(Company.NameMaxLength)
            .IsRequired();

        builder.Property(company => company.Website)
            .HasMaxLength(Company.WebsiteMaxLength);

        builder.Property(company => company.NormalizedWebsite)
            .HasMaxLength(Company.WebsiteMaxLength);

        builder.Property(company => company.Location)
            .HasMaxLength(Company.LocationMaxLength);

        builder.HasIndex(company => new { company.UserId, company.NormalizedName })
            .IsUnique();

        builder.HasOne(company => company.User)
            .WithMany(user => user.Companies)
            .HasForeignKey(company => company.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(company => company.JobApplications)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
