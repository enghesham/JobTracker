namespace JobTracker.Application.Features.Companies.Common;

public interface ICompanyReadService
{
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);
}
