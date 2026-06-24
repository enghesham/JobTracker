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
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(company => company.NormalizedName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(company => company.Website)
            .HasMaxLength(500);

        builder.Property(company => company.NormalizedWebsite)
            .HasMaxLength(500);

        builder.Property(company => company.Location)
            .HasMaxLength(200);

        builder.HasIndex(company => new { company.UserId, company.NormalizedName })
            .IsUnique();

        builder.HasOne(company => company.User)
            .WithMany(user => user.Companies)
            .HasForeignKey(company => company.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
