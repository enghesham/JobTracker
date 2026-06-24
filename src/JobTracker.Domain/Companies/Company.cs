using JobTracker.Domain.Common;
using JobTracker.Domain.JobApplications;
using JobTracker.Domain.Users;

namespace JobTracker.Domain.Companies;

public sealed class Company : BaseEntity
{
    public const int NameMaxLength = 200;
    public const int WebsiteMaxLength = 500;
    public const int LocationMaxLength = 200;

    private readonly List<JobApplication> _jobApplications = new();

    private Company() { }

    public Company(Guid userId, string name, string? website, string? location)
    {
        DomainGuard.AgainstEmpty(userId, nameof(userId));

        UserId = userId;
        Name = DomainGuard.Required(name, nameof(name), NameMaxLength);
        NormalizedName = NormalizeName(Name);
        Website = DomainGuard.OptionalHttpUrl(website, nameof(website), WebsiteMaxLength);
        NormalizedWebsite = NormalizeWebsite(Website);
        Location = DomainGuard.Optional(location, nameof(location), LocationMaxLength);
    }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? NormalizedWebsite { get; private set; }
    public string? Location { get; private set; }

    public IReadOnlyCollection<JobApplication> JobApplications => _jobApplications;

    public static string NormalizeName(string name) =>
        DomainGuard.Required(name, nameof(name), NameMaxLength).ToUpperInvariant();

    public static string? NormalizeWebsite(string? website)
    {
        var normalized = DomainGuard.OptionalHttpUrl(website, nameof(website), WebsiteMaxLength);
        return normalized?.ToLowerInvariant();
    }
}
