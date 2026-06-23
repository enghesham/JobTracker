using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Infrastructure.Authentication;
using JobTracker.Infrastructure.BackgroundJobs;
using JobTracker.Infrastructure.Persistence;
using JobTracker.Infrastructure.Persistence.ReadServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.AddScoped<ICompanyReadService, CompanyReadService>();
        services.AddScoped<IJobApplicationReadService, JobApplicationReadService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddHostedService<FollowUpReminderWorker>();

        return services;
    }
}
