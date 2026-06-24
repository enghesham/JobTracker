using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Companies.Common;
using MediatR;

namespace JobTracker.Application.Features.Companies.GetAll;

public sealed record GetCompaniesQuery : IRequest<IReadOnlyCollection<CompanyDto>>;

public sealed class GetCompaniesQueryHandler(
    ICompanyReadService companyReadService,
    ICurrentUserService currentUserService) : IRequestHandler<GetCompaniesQuery, IReadOnlyCollection<CompanyDto>>
{
    public Task<IReadOnlyCollection<CompanyDto>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        return companyReadService.GetCompaniesAsync(userId, cancellationToken);
    }
}
