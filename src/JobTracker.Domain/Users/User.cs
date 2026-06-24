using JobTracker.Domain.Common;
using JobTracker.Domain.Companies;
using JobTracker.Domain.JobApplications;

namespace JobTracker.Domain.Users;

public sealed class User : BaseEntity
{
    public const int FullNameMaxLength = 200;
    public const int EmailMaxLength = 320;
    public const int PasswordHashMaxLength = 500;

    private readonly List<Company> _companies = new();
    private readonly List<JobApplication> _jobApplications = new();

    private User() { }

    public User(string fullName, string email, string passwordHash)
    {
        FullName = DomainGuard.Required(fullName, nameof(fullName), FullNameMaxLength);
        Email = NormalizeEmail(email);
        PasswordHash = DomainGuard.Required(passwordHash, nameof(passwordHash), PasswordHashMaxLength);
    }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public IReadOnlyCollection<Company> Companies => _companies;
    public IReadOnlyCollection<JobApplication> JobApplications => _jobApplications;

    public static string NormalizeEmail(string email) =>
        DomainGuard.Required(email, nameof(email), EmailMaxLength).ToLowerInvariant();
}
