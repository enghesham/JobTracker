using FluentAssertions;
using JobTracker.Domain.Users;
using JobTracker.Infrastructure.Persistence;
using JobTracker.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Tests;

public sealed class TimeProviderTests
{
    [Fact]
    public async Task SaveChangesAsync_uses_configured_time_provider_for_created_timestamp()
    {
        var utcNow = new DateTimeOffset(2026, 6, 24, 10, 30, 0, TimeSpan.Zero);
        var timeProvider = new FixedTimeProvider(utcNow);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditingSaveChangesInterceptor(timeProvider))
            .Options;

        await using var dbContext = new ApplicationDbContext(options);
        var user = new User("Test User", "test@example.com", "hashed-password");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        user.CreatedAtUtc.Should().Be(utcNow);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
