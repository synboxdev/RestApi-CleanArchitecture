using FluentValidation;
using Hestia.Application.Handlers.Validation;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Interfaces.Product;
using Hestia.Application.Interfaces.Response;
using Hestia.Application.Services;
using Hestia.Application.Services.Authentication;
using Hestia.Application.Services.Product;
using MediatR;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Hestia.Api.Configuration.Application;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddAuthenticationServices();
        services.AddApiServices();
        services.AddApplicationServices();

        // MediatR
        services.AddMediatR();

        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // FluentValidation validators
        services.AddFluentValidationValidators();

        // MediatR layers
        services.AddTransient<IAccessLayer, MediatrAdapter>();
        services.AddTransient<ICoreLayer, MediatrAdapter>();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });

        return services;
    }

    public static IServiceCollection AddFluentValidationValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.Load("Hestia.Domain"));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddTransient<IUserService, UserService>();

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddTransient<IResponseService, ResponseService>();

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IProductService, ProductService>();

        return services;
    }
}