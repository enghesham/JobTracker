using JobTracker.Application.Companies;
using JobTracker.Application.Common.Interfaces;
using MediatR;

namespace JobTracker.Application.Companies.Queries.GetCompanies;

public sealed record GetCompaniesQuery : IRequest<IReadOnlyCollection<CompanyDto>>;

public sealed class GetCompaniesQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCompaniesQuery, IReadOnlyCollection<CompanyDto>>
{
    public Task<IReadOnlyCollection<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<CompanyDto> companies = dbContext.Companies
            .OrderBy(company => company.Name)
            .Select(company => new CompanyDto(company.Id, company.Name, company.Website, company.Location))
            .ToArray();

        return Task.FromResult(companies);
    }
}
