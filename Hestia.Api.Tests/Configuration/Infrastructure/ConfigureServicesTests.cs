using Hestia.Background;
using Hestia.Background.Interfaces;
using Hestia.Background.Tasks;
using Hestia.Persistence.Contexts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hestia.Api.Tests.Configuration.Infrastructure;

public class ConfigureServicesTests
{
    [Fact]
    public void ConfigureInfrastructuresServices_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.ConfigureInfrastructuresServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Check if database contexts are registered
        var hestiaContext = serviceProvider.GetService<HestiaContext>();
        Assert.NotNull(hestiaContext);
        var rheaContext = serviceProvider.GetService<RheaContext>();
        Assert.NotNull(rheaContext);

        // Check if memory cache is registered
        var memoryCache = serviceProvider.GetService<IMemoryCache>();
        Assert.NotNull(memoryCache);

        // Check if background runners are registered
        var tokenCleanupTask = serviceProvider.GetService<TokenCleanupTask>();
        Assert.NotNull(tokenCleanupTask);
        var scheduledTask = serviceProvider.GetService<IScheduledTask>();
        Assert.NotNull(scheduledTask);
        var hostedService = serviceProvider.GetService<IHostedService>();
        Assert.NotNull(hostedService);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureInfrastructuresServices(this IServiceCollection services)
    {
        services.AddScoped<HestiaContext>();
        services.AddScoped<RheaContext>();

        services.AddMemoryCache();
        services.ConfigureBackgroundRunners();

        return services;
    }

    public static IServiceCollection ConfigureBackgroundRunners(this IServiceCollection services)
    {
        services.AddScoped<TokenCleanupTask>();
        services.AddScoped<IScheduledTask, TokenCleanupTask>();
        services.AddSingleton<IHostedService, BackgroundRunnerService>();

        return services;
    }
}