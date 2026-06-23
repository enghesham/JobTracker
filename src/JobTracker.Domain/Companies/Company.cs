using JobTracker.Domain.Common;
using JobTracker.Domain.JobApplications;

namespace JobTracker.Domain.Companies;

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
