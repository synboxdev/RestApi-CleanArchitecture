using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Authentication;
using Hestia.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Hestia.Api.Tests.Configuration.Extensions;

public class WebApplicationBuilderExtensionsTests
{
    [Fact]
    public void ConfigureSettings_ShouldAddConfigurationSettings()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = "Development";

        // Act
        builder.ConfigureSettings();

        // Assert
        var configuration = builder.Configuration;
        var jwtSettings = configuration.GetSection("Jwt").Get<Jwt>();
        Assert.NotNull(jwtSettings);
    }

    [Fact]
    public void ConfigureAuthentication_ShouldAddAuthenticationServices()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddScoped<HestiaContext>();
        builder.Environment.EnvironmentName = "Development";
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");

        var jwtSettings = new Jwt
        {
            ValidIssuer = "testIssuer",
            ValidAudience = "testAudience",
            Secret = "verySecretKey123!"
        };

        builder.Configuration.GetSection("Jwt").Bind(jwtSettings);

        builder.Host.ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;
            services.Configure<Jwt>(configuration.GetSection("Jwt"));
        });

        // Act
        builder.ConfigureAuthentication();

        // Assert
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Check if Identity services are registered
        var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
        Assert.NotNull(userManager);
        var signInManager = serviceProvider.GetService<SignInManager<ApplicationUser>>();
        Assert.NotNull(signInManager);

        // Check if Authentication services are registered
        var authenticationSchemeProvider = serviceProvider.GetService<IAuthenticationSchemeProvider>();
        Assert.NotNull(authenticationSchemeProvider);
        var jwtBearerHandler = serviceProvider.GetService<JwtBearerHandler>();
        Assert.NotNull(jwtBearerHandler);
    }
}

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureSettings(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        builder.Host.ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;
            services.Configure<Jwt>(configuration.GetSection("Jwt"));
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        var jwtCredentials = builder.Configuration.GetSection("Jwt").Get<Jwt>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<HestiaContext>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.IncludeErrorDetails = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtCredentials.ValidIssuer,
                ValidAudience = jwtCredentials.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCredentials.Secret))
            };
        });

        return builder;
    }
}