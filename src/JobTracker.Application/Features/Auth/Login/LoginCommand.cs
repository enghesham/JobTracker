using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Auth;
using MediatR;

namespace JobTracker.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public sealed class LoginCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = dbContext.Users.FirstOrDefault(candidate => candidate.Email == email);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return Task.FromResult(new AuthResponse(user.Id, user.FullName, user.Email, jwtTokenService.CreateToken(user)));
    }
}
