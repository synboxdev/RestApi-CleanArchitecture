using AutoMapper;
using Hestia.Access.Requests.Shared;
using Hestia.Access.Requests.User.Queries.UserExists;
using Hestia.Access.Requests.User.Queries.ValidateUserLogin;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Domain.Enumerations;
using Hestia.Domain.Models.Authentication;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Hestia.Application.Services.Authentication;

public class AuthService(
    ITokenService tokenService,
    IUserService userService,
    IMapper mapper,
    IAccessLayer accessLayer,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<(ApplicationUserLoginResponseDto, HttpStatusCode)> LoginAsync(ApplicationUserLoginDto model, CancellationToken cancellationToken = default)
    {
        string? token = null!;
        ApplicationUser? existingUser = null;
        var statusCode = HttpStatusCode.NoContent;

        try
        {
            existingUser = await GetExistingUserAsync(model.Username, model.Email, cancellationToken);

            if (existingUser is null)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            else if (await ValidateUserLoginAsync(existingUser, model.Password, cancellationToken))
            {
                statusCode = HttpStatusCode.OK;
                token = await tokenService.GenerateTokenAsync(existingUser, cancellationToken);
                await accessLayer.ExecuteAsync(new ExecuteSaveChangesAsync(), cancellationToken);
            }
            else
            {
                statusCode = HttpStatusCode.BadRequest;
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occurred during login: {ex.Message}");
            statusCode = HttpStatusCode.InternalServerError;
        }

        var response = new ApplicationUserLoginResponseDto { Username = model.Username, Email = model.Email, Token = token };
        return (response, statusCode);
    }

    public async Task<(ApplicationUserRegisterResponseDto, HttpStatusCode)> RegisterAsync(ApplicationUserRegisterDto model, Role? role = null, CancellationToken cancellationToken = default)
    {
        ApplicationUser? existingUser = null;
        var statusCode = HttpStatusCode.NoContent;

        try
        {
            existingUser = await GetExistingUserAsync(model.Username, model.Email, cancellationToken);

            if (existingUser is null)
            {
                var applicationUser = await userService.CreateUserAsync(model, cancellationToken);

                if (applicationUser is not null)
                {
                    existingUser = applicationUser;
                    statusCode = HttpStatusCode.Created;
                }
            }
            else
                statusCode = HttpStatusCode.Found;
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occurred during creation of new User: {ex.Message}");
            statusCode = HttpStatusCode.InternalServerError;
        }

        var response = mapper.Map<ApplicationUserRegisterResponseDto>(existingUser);
        return (response, statusCode);
    }

    private async Task<ApplicationUser?> GetExistingUserAsync(string Username, string Email, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new GetExistingUserQuery(Username, Email), cancellationToken);

    private async Task<bool> ValidateUserLoginAsync(ApplicationUser User, string LoginPassword, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new ValidateUserLoginQuery(User, LoginPassword), cancellationToken);
}