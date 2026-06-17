using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
