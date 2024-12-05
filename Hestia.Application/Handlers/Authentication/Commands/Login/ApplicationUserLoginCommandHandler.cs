using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using MediatR;
using System.Net;

namespace Hestia.Application.Handlers.Authentication.Commands.Login;

public class ApplicationUserLoginCommandHandler(IAuthService authService) : IRequestHandler<ApplicationUserLoginCommand, ApiResponse<ApplicationUserLoginResponseDto>>
{
    public async Task<ApiResponse<ApplicationUserLoginResponseDto>> Handle(ApplicationUserLoginCommand request, CancellationToken cancellationToken)
    {
        var (loginResponse, statusCode) = await authService.LoginAsync(request.Model, cancellationToken);
        string message = GetMessage(loginResponse, statusCode);
        return new ApiResponse<ApplicationUserLoginResponseDto>(statusCode, message, loginResponse);
    }

    public static string GetMessage(ApplicationUserLoginResponseDto? loginResponse, HttpStatusCode statusCode) =>
        loginResponse is null || statusCode == HttpStatusCode.InternalServerError ?
        $"Error occurred during login!" :
        statusCode switch
        {
            HttpStatusCode.OK => $"Login for the user '{loginResponse.Username}' was successful!",
            HttpStatusCode.NotFound => $"User by the username '{loginResponse.Username}' does not exist! Make sure that provided credentials are correct!",
            HttpStatusCode.BadRequest => $"Invalid password provided for user '{loginResponse.Username}'!"
        };
}