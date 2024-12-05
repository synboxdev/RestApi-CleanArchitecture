using Hestia.Api.Configuration.Api;
using Hestia.Api.Configuration.Application;
using Hestia.Api.Configuration.Infrastructure;
using Hestia.Api.Middleware;
using Hestia.Persistence.Contexts;
using Hestia.Persistence.Contexts.Extensions;

namespace Hestia.Api.Configuration.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApiServices();
        builder.Services.ConfigureInfrastructuresServices();
        builder.Services.ConfigureApplicationServices();
        builder.ConfigureAuthentication();
        return builder;
    }

    public static WebApplication ConfigurePostBuildApplication(this WebApplication app)
    {
        app.ConfigureSwagger();
        app.ConfigureDbContextMigrations();
        app.UseExceptionMiddleware();
        app.UseHttpsRedirection();
        app.UseStatusCodePages();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static WebApplication ConfigureSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hestia RestApi");
            options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
            options.DocumentTitle = $"Hestia API ({app.Environment.EnvironmentName})";
        });
        app.UseStaticFiles();

        return app;
    }

    private static WebApplication ConfigureDbContextMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var hestiaContext = services.GetRequiredService<HestiaContext>();
            var rheaContext = services.GetRequiredService<RheaContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            hestiaContext.EnsureMigrationsApplied(logger);
            rheaContext.EnsureMigrationsApplied(logger);
        }

        return app;
    }
}