using JobTracker.Application.Features.Auth.Common;
using JobTracker.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence.Stores;

public sealed class UserStore(ApplicationDbContext dbContext) : IUserStore
{
    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }
}

