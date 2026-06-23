using JobTracker.Application.Features.Companies.Common;
using MediatR;

namespace JobTracker.Application.Features.Companies.GetAll;

public sealed record GetCompaniesQuery : IRequest<IReadOnlyCollection<CompanyDto>>;

public sealed class GetCompaniesQueryHandler(ICompanyReadService companyReadService)
    : IRequestHandler<GetCompaniesQuery, IReadOnlyCollection<CompanyDto>>
{
    public Task<IReadOnlyCollection<CompanyDto>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        return companyReadService.GetCompaniesAsync(cancellationToken);
    }
}
