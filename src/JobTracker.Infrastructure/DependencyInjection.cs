using JobTracker.Application.Common.Interfaces;
using JobTracker.Infrastructure.Auth;
using JobTracker.Infrastructure.BackgroundJobs;
using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // var connectionString = configuration.GetConnectionString("DefaultConnection")
        //     ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");
        var provider = configuration["Database:Provider"];

        if (provider == "PostgreSql")
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));
        }
        else if (provider == "SqlServer")
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServer")));
        }
        else
        {
            throw new InvalidOperationException("Unsupported database provider.");
        }

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddHostedService<FollowUpReminderWorker>();

        return services;

        }
}
