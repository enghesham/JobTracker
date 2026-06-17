namespace JobTracker.Application.Auth.Commands;

public sealed record AuthResponse(Guid UserId, string FullName, string Email, string Token);
