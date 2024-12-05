using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Hestia.Api.Configuration.Api;

public static class ConfigureServices
{
    public static IServiceCollection ConfigureApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        AddSwaggerGen(services);

        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddSwaggerGen(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"Hestia RestApi",
                Description = "",
                Version = "v1"
            });

            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
}