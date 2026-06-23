namespace JobTracker.Application.Features.Auth;

public sealed record AuthResponse(Guid UserId, string FullName, string Email, string Token);
