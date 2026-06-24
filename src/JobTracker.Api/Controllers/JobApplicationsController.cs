using JobTracker.Api.Extensions;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Application.Features.JobApplications.Create;
using JobTracker.Application.Features.JobApplications.GetMine;
using JobTracker.Application.Features.JobApplications.UpdateStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class JobApplicationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<JobApplicationDto>>> GetMine(CancellationToken cancellationToken)
    {
        var applications = await mediator.Send(new GetMyJobApplicationsQuery(), cancellationToken);
        return this.ToActionResult(applications);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationDto>> Create(CreateJobApplicationCommand command, CancellationToken cancellationToken)
    {
        var application = await mediator.Send(command, cancellationToken);
        return this.CreatedAtActionResult(
            application,
            nameof(GetMine),
            application.IsSuccess ? new { id = application.Value.Id } : null);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<JobApplicationDto>> UpdateStatus(
        Guid id,
        UpdateJobApplicationStatusRequest request,
        CancellationToken cancellationToken)
    {
        var application = await mediator.Send(
            new UpdateJobApplicationStatusCommand(id, request.Status),
            cancellationToken);

        return this.ToActionResult(application);
    }
}
