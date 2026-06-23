using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Domain.Companies;
using MediatR;

namespace JobTracker.Application.Features.Companies.Create;

public sealed record CreateCompanyCommand(string Name, string? Website, string? Location) : IRequest<CompanyDto>;

public sealed class CreateCompanyCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Company(
            request.Name.Trim(),
            string.IsNullOrWhiteSpace(request.Website) ? null : request.Website.Trim(),
            string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim());

        dbContext.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompanyDto(company.Id, company.Name, company.Website, company.Location);
    }
}
