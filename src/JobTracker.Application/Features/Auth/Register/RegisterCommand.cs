using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Auth;
using JobTracker.Domain.Users;
using MediatR;

namespace JobTracker.Application.Features.Auth.Register;

public sealed record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResponse>;

public sealed class RegisterCommandHandler(
    IUserStore userStore,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = User.NormalizeEmail(request.Email);

        if (await userStore.EmailExistsAsync(email, cancellationToken))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User(
            request.FullName,
            email,
            passwordHasher.Hash(request.Password));

        userStore.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(user.Id, user.FullName, user.Email, jwtTokenService.CreateToken(user));
    }
}
