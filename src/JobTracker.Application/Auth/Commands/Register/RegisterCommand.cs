using JobTracker.Application.Auth.Commands;
using JobTracker.Application.Common.Interfaces;
using JobTracker.Domain.Entities;
using MediatR;

namespace JobTracker.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResponse>;

public sealed class RegisterCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (dbContext.Users.Any(user => user.Email == email))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User(
            request.FullName.Trim(),
            email,
            passwordHasher.Hash(request.Password)
        );

        dbContext.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.FullName, user.Email, jwtTokenService.CreateToken(user));
    }
}
