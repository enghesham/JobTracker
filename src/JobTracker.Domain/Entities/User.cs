using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities;

public sealed class User : BaseEntity
{
    private User() { }
    public User(string fullName, string email, string passwordHash)
    {
        FullName = fullName;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
    }
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public ICollection<Company> Companies { get; private set; } = new List<Company>();
    public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();
}
