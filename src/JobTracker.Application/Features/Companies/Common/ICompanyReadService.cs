namespace JobTracker.Application.Features.Companies.Common;

public interface ICompanyReadService
{
    Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync(CancellationToken cancellationToken = default);
}
