using JobTracker.Domain.Companies;

namespace JobTracker.Application.Features.Companies.Common;

public interface ICompanyStore
{
    Task<bool> ExistsForUserAsync(Guid userId, string normalizedName, CancellationToken cancellationToken = default);
    Task<Company?> GetForUserAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default);
    void Add(Company company);
}
