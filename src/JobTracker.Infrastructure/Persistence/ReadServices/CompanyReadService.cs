using JobTracker.Application.Features.Companies.Common;
using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.ReadServices;

public sealed class CompanyReadService(ApplicationDbContext dbContext) : ICompanyReadService
{
    public async Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Companies
            .AsNoTracking()
            .OrderBy(company => company.Name)
            .Select(company => new CompanyDto(
                company.Id,
                company.Name,
                company.Website,
                company.Location))
            .ToArrayAsync(cancellationToken);
    }
}
