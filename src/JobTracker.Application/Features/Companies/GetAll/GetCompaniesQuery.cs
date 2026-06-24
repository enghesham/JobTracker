using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Common.Results;
using JobTracker.Application.Features.Companies.Common;
using MediatR;

namespace JobTracker.Application.Features.Companies.GetAll;

public sealed record GetCompaniesQuery : IRequest<Result<IReadOnlyCollection<CompanyDto>>>;

public sealed class GetCompaniesQueryHandler(
    ICompanyReadService companyReadService,
    ICurrentUserService currentUserService) : IRequestHandler<GetCompaniesQuery, Result<IReadOnlyCollection<CompanyDto>>>
{
    public async Task<Result<IReadOnlyCollection<CompanyDto>>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<IReadOnlyCollection<CompanyDto>>.Failure(Error.Unauthorized(
                "user-not-authenticated",
                "User is not authenticated",
                "The current request does not contain an authenticated user."));
        }

        var companies = await companyReadService.GetCompaniesAsync(userId.Value, cancellationToken);
        return Result<IReadOnlyCollection<CompanyDto>>.Success(companies);
    }
}
