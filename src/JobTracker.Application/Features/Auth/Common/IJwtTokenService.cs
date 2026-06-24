using JobTracker.Domain.Users;

namespace JobTracker.Application.Features.Auth.Common;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
