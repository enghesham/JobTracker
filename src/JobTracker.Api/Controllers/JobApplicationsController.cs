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
        return Ok(applications);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationDto>> Create(CreateJobApplicationCommand command, CancellationToken cancellationToken)
    {
        var application = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMine), new { id = application.Id }, application);
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

        return Ok(application);
    }
}
