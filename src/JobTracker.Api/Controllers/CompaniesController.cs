using JobTracker.Api.Extensions;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Application.Features.Companies.Create;
using JobTracker.Application.Features.Companies.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CompanyDto>>> GetCompanies(CancellationToken cancellationToken)
    {
        var companies = await mediator.Send(new GetCompaniesQuery(), cancellationToken);
        return this.ToActionResult(companies);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var company = await mediator.Send(command, cancellationToken);
        return this.CreatedAtActionResult(
            company,
            nameof(GetCompanies),
            company.IsSuccess ? new { id = company.Value.Id } : null);
    }
}
