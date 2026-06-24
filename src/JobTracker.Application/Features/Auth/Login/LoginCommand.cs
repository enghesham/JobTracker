using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Auth;
using JobTracker.Domain.Users;
using MediatR;

namespace JobTracker.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public sealed class LoginCommandHandler(
    IUserStore userStore,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = User.NormalizeEmail(request.Email);
        var user = await userStore.GetByEmailAsync(email, cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return new AuthResponse(user.Id, user.FullName, user.Email, jwtTokenService.CreateToken(user));
    }
}
