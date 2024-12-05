using Hestia.Background;
using Hestia.Background.Interfaces;
using Hestia.Background.Tasks;
using Hestia.Persistence.Contexts;

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
        services.AddScoped<HestiaContext>();
        services.AddScoped<RheaContext>();

        return services;
    }
}