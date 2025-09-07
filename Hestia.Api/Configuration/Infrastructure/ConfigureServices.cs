using Hestia.Background;
using Hestia.Background.Interfaces;
using Hestia.Background.Tasks;
using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Hestia.Api.Configuration.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureInfrastructuresServices(this IServiceCollection services)
    {
        services.ConfigureDbContexts();
        services.AddMemoryCache();
        services.ConfigureBackgroundRunners();

        return services;
    }

    private static IServiceCollection ConfigureBackgroundRunners(this IServiceCollection services)
    {
        services.AddScoped<TokenCleanupTask>();
        services.AddScoped<IScheduledTask, TokenCleanupTask>();
        services.AddSingleton<IHostedService, BackgroundRunnerService>();

        return services;
    }

    private static IServiceCollection ConfigureDbContexts(this IServiceCollection services)
    {
        services.AddDbContext<HestiaContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configuration.GetConnectionString("AuthServer"));
        });

        services.AddDbContext<RheaContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configuration.GetConnectionString("DataServer"));
        });

        return services;
    }
}