using JobTracker.Domain.Common;
using JobTracker.Domain.JobApplications;
using JobTracker.Domain.Users;

namespace JobTracker.Domain.Companies;

public sealed class Company : BaseEntity
{
    private Company() { }

    public Company(Guid userId, string name, string? website, string? location)
    {
        UserId = userId;
        Name = name.Trim();
        NormalizedName = NormalizeName(name);
        Website = NormalizeWebsiteForDisplay(website);
        NormalizedWebsite = NormalizeWebsite(website);
        Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? NormalizedWebsite { get; private set; }
    public string? Location { get; private set; }

    public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

    public static string NormalizeName(string name) =>
        name.Trim().ToUpperInvariant();

    public static string? NormalizeWebsite(string? website)
    {
        if (string.IsNullOrWhiteSpace(website))
        {
            return null;
        }

        return website.Trim().TrimEnd('/').ToLowerInvariant();
    }

    private static string? NormalizeWebsiteForDisplay(string? website)
    {
        if (string.IsNullOrWhiteSpace(website))
        {
            return null;
        }

        return website.Trim().TrimEnd('/');
    }
}
