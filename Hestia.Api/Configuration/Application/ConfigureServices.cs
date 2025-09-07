using FluentValidation;
using Hestia.Application.Handlers.Validation;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Product;
using Hestia.Application.Interfaces.Response;
using Hestia.Application.Services;
using Hestia.Application.Services.Authentication;
using Hestia.Application.Services.Product;
using Hestia.Mediator.Infrastructure;
using Hestia.Mediator.Infrastructure.Layers;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Pipeline;
using Hestia.Mediator.Services;
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

        // Mediator pattern
        services.AddMediator();

        // FluentValidation validators
        services.AddFluentValidationValidators();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        // Mediator pattern service
        services.AddSingleton<IMediator, MediatorService>();

        // Mediator pattern layers
        services.AddTransient<IAccessLayer, MediatorAdapter>();
        services.AddTransient<ICoreLayer, MediatorAdapter>();

        services.AddRequestHandlers();

        return services;
    }

    private static void AddRequestHandlers(this IServiceCollection services)
    {
        var handlerInterfaceType = typeof(IRequestHandler<,>);

        var assemblies = Directory
            .GetFiles(AppContext.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(file =>
            {
                string name = Path.GetFileName(file);
                // Adjust filter to match your solution's assemblies
                return name.StartsWith("Hestia.", StringComparison.OrdinalIgnoreCase);
            })
            .Select(Assembly.LoadFrom)
            .ToList();

        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(x => x.iface.IsGenericType &&
                        x.iface.GetGenericTypeDefinition() == handlerInterfaceType)
            .ToList();

        foreach (var handler in handlerTypes)
        {
            services.AddTransient(handler.iface, handler.type);
        }
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