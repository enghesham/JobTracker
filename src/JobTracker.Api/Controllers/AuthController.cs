using JobTracker.Api.Extensions;
using JobTracker.Application.Features.Auth;
using JobTracker.Application.Features.Auth.Login;
using JobTracker.Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return this.CreatedAtActionResult(
            response,
            nameof(Register),
            response.IsSuccess ? new { id = response.Value.UserId } : null);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(response);
    }
}
