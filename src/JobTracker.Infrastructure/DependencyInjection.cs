using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Infrastructure.Authentication;
using JobTracker.Infrastructure.BackgroundJobs;
using JobTracker.Infrastructure.Persistence;
using JobTracker.Infrastructure.Persistence.ReadServices;
using JobTracker.Infrastructure.Persistence.Stores;
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
                options.UseNpgsql(GetRequiredConnectionString(configuration, "PostgreSql")));
        }
        else if (provider == "SqlServer")
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(GetRequiredConnectionString(configuration, "SqlServer")));
        }
        else
        {
            throw new InvalidOperationException("Unsupported database provider.");
        }

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUserStore, UserStore>();
        services.AddScoped<ICompanyStore, CompanyStore>();
        services.AddScoped<IJobApplicationStore, JobApplicationStore>();

        services.AddScoped<ICompanyReadService, CompanyReadService>();
        services.AddScoped<IJobApplicationReadService, JobApplicationReadService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddHostedService<FollowUpReminderWorker>();

        return services;
    }

    private static string GetRequiredConnectionString(IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name)
            ?? throw new InvalidOperationException(
                $"ConnectionStrings:{name} is not configured. Use User Secrets for local development or an environment variable such as ConnectionStrings__{name}.");
    }
}

