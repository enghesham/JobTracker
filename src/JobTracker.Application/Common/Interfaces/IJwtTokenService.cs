using JobTracker.Domain.Entities;

namespace JobTracker.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
