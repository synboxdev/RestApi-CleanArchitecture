using Hestia.Api.Configuration.Api;
using Hestia.Api.Tests.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;

namespace Hestia.Api.Tests.Configuration.Api;

public class ConfigureServicesTests
{
    [Fact]
    public void ConfigureApiServices_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        // Add necessary services
        services.AddLogging();
        services.AddSingleton<IHostEnvironment>(new MockHostEnvironment { EnvironmentName = Environments.Development });
        services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment { EnvironmentName = Environments.Development });

        // Act
        services.ConfigureApiServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Check if MVC services are registered
        var controllerFactory = serviceProvider.GetService<IControllerFactory>();
        Assert.NotNull(controllerFactory);

        // Check if ProblemDetails are registered
        var problemDetailsFactory = serviceProvider.GetService<ProblemDetailsFactory>();
        Assert.NotNull(problemDetailsFactory);

        // Check if SwaggerGen is registered
        var swaggerProvider = serviceProvider.GetService<ISwaggerProvider>();
        Assert.NotNull(swaggerProvider);
    }
}