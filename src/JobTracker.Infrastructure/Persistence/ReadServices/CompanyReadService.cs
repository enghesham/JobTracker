using JobTracker.Application.Features.Companies.Common;
using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.ReadServices;

public sealed class CompanyReadService(ApplicationDbContext dbContext) : ICompanyReadService
{
    public async Task<IReadOnlyCollection<CompanyDto>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Companies
            .AsNoTracking()
            .Where(company => company.UserId == userId)
            .OrderBy(company => company.Name)
            .Select(company => new CompanyDto(
                company.Id,
                company.Name,
                company.Website,
                company.Location))
            .ToArrayAsync(cancellationToken);
    }
}
