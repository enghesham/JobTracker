using JobTracker.Domain.Users;

namespace JobTracker.Application.Common.Interfaces;

public interface IUserStore
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
}
