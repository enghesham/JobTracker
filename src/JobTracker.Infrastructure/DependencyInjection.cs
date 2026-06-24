using JobTracker.Application.Common.Interfaces;
using JobTracker.Application.Features.Companies.Common;
using JobTracker.Application.Features.JobApplications.Common;
using JobTracker.Infrastructure.Authentication;
using JobTracker.Infrastructure.BackgroundJobs;
using JobTracker.Infrastructure.Persistence;
using JobTracker.Infrastructure.Persistence.Interceptors;
using JobTracker.Infrastructure.Persistence.ReadServices;
using JobTracker.Infrastructure.Persistence.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<AuditingSaveChangesInterceptor>();

        var provider = configuration["Database:Provider"];

        if (provider == "PostgreSql")
        {
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
                options
                    .UseNpgsql(GetRequiredConnectionString(configuration, "PostgreSql"))
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>()));
        }
        else if (provider == "SqlServer")
        {
            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
                options
                    .UseSqlServer(GetRequiredConnectionString(configuration, "SqlServer"))
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditingSaveChangesInterceptor>()));
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
