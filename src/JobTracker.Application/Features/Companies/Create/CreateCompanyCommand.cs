using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Common.Results;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Domain.Companies;
using MediatR;

namespace JobTracker.Application.Features.Companies.Create;

public sealed record CreateCompanyCommand(string Name, string? Website, string? Location) : IRequest<Result<CompanyDto>>;

public sealed class CreateCompanyCommandHandler(
    ICompanyStore companyStore,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
{
    public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result<CompanyDto>.Failure(Error.Unauthorized(
                "user-not-authenticated",
                "User is not authenticated",
                "The current request does not contain an authenticated user."));
        }

        var normalizedName = Company.NormalizeName(request.Name);

        if (await companyStore.ExistsForUserAsync(userId.Value, normalizedName, cancellationToken))
        {
            return Result<CompanyDto>.Failure(Error.Conflict(
                "company-already-exists",
                "Company already exists",
                "A company with the same name already exists for the current user."));
        }

        var company = new Company(
            userId.Value,
            request.Name,
            request.Website,
            request.Location);

        companyStore.Add(company);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CompanyDto(company.Id, company.Name, company.Website, company.Location);
    }
}
