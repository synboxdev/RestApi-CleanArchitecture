using FluentValidation;
using Hestia.Api.Configuration.Application;
using Hestia.Api.Tests.Shared;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Product;
using Hestia.Application.Interfaces.Response;
using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure;
using Hestia.Mediator.Infrastructure.Layers;
using Hestia.Persistence.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Hestia.Api.Tests.Configuration.Application;

public class ConfigureServicesTests
{
    [Fact]
    public void ConfigureApplicationServices_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Add necessary services
        services.AddLogging();
        services.AddSingleton<IWebHostEnvironment>(new MockWebHostEnvironment { EnvironmentName = Environments.Development });
        services.AddMemoryCache();
        services.AddDataProtection();
        services.AddDbContext<HestiaContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });

        // Add Identity services
        services.AddIdentityCore<ApplicationUser>(options => { })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<HestiaContext>()
            .AddDefaultTokenProviders();

        // Act
        services.ConfigureApplicationServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Check if authentication services are registered
        var authService = serviceProvider.GetService<IAuthService>();
        Assert.NotNull(authService);
        var tokenService = serviceProvider.GetService<ITokenService>();
        Assert.NotNull(tokenService);
        var userService = serviceProvider.GetService<IUserService>();
        Assert.NotNull(userService);

        // Check if API services are registered
        var responseService = serviceProvider.GetService<IResponseService>();
        Assert.NotNull(responseService);

        // Check if application services are registered
        var productService = serviceProvider.GetService<IProductService>();
        Assert.NotNull(productService);

        // Check if MediatR is registered
        var mediator = serviceProvider.GetService<IMediator>();
        Assert.NotNull(mediator);

        // Check if FluentValidation validators are registered
        var validators = serviceProvider.GetServices<IValidator>();
        Assert.NotNull(validators);

        // Check if MediatR layers are registered
        var accessLayer = serviceProvider.GetService<IAccessLayer>();
        Assert.NotNull(accessLayer);
        var coreLayer = serviceProvider.GetService<ICoreLayer>();
        Assert.NotNull(coreLayer);

        // Check if controllers are registered
        var controllerFactory = serviceProvider.GetService<IControllerFactory>();
        Assert.NotNull(controllerFactory);

        // Check if JSON options are configured correctly
        var mvcOptions = serviceProvider.GetService<IOptions<MvcOptions>>().Value;
        var jsonOptions = mvcOptions.InputFormatters.OfType<SystemTextJsonInputFormatter>().First().SerializerOptions;
        var enumConverter = jsonOptions.Converters.OfType<JsonStringEnumConverter>().FirstOrDefault();
        Assert.NotNull(enumConverter);
    }
}