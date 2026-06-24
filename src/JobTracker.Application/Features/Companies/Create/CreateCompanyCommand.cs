using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Domain.Companies;
using MediatR;

namespace JobTracker.Application.Features.Companies.Create;

public sealed record CreateCompanyCommand(string Name, string? Website, string? Location) : IRequest<CompanyDto>;

public sealed class CreateCompanyCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var normalizedName = Company.NormalizeName(request.Name);

        if (dbContext.Companies.Any(company => company.UserId == userId && company.NormalizedName == normalizedName))
        {
            throw new InvalidOperationException("Company already exists for the current user.");
        }

        var company = new Company(
            userId,
            request.Name,
            request.Website,
            request.Location);

        dbContext.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompanyDto(company.Id, company.Name, company.Website, company.Location);
    }
}
