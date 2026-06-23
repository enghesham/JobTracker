using JobTracker.Domain.Users;

namespace JobTracker.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
