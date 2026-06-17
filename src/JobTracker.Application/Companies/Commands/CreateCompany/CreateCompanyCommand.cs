using JobTracker.Application.Companies;
using JobTracker.Application.Common.Interfaces;
using JobTracker.Domain.Entities;
using MediatR;

namespace JobTracker.Application.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(string Name, string? Website, string? Location) : IRequest<CompanyDto>;

public sealed class CreateCompanyCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Company
        {
            Name = request.Name.Trim(),
            Website = string.IsNullOrWhiteSpace(request.Website) ? null : request.Website.Trim(),
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim()
        };

        dbContext.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompanyDto(company.Id, company.Name, company.Website, company.Location);
    }
}
