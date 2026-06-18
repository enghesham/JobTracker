using JobTracker.Domain.Common;

namespace JobTracker.Domain.Entities;

public sealed class Company : BaseEntity
{
    private Company() { }
    public Company(string name, string? website, string? location)
    {
        Name = name;
        Website = website;
        Location = location;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Website { get; private set; }
    public string? Location { get; private set; }

    public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();
}
