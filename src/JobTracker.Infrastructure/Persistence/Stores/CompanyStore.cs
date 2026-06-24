using JobTracker.Application.Common.Interfaces;
using JobTracker.Domain.Companies;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.Stores;

public sealed class CompanyStore(ApplicationDbContext dbContext) : ICompanyStore
{
    public Task<bool> ExistsForUserAsync(Guid userId, string normalizedName, CancellationToken cancellationToken = default)
    {
        return dbContext.Companies.AnyAsync(
            company => company.UserId == userId && company.NormalizedName == normalizedName,
            cancellationToken);
    }

    public Task<Company?> GetForUserAsync(Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Companies.FirstOrDefaultAsync(
            company => company.Id == companyId && company.UserId == userId,
            cancellationToken);
    }

    public void Add(Company company)
    {
        dbContext.Companies.Add(company);
    }
}
