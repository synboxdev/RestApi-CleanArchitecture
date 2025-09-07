using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using Hestia.Mediator.Infrastructure.Messaging;
using System.Net;

namespace Hestia.Application.Handlers.Authentication.Commands.Register;

public class ApplicationUserRegisterCommandHandler(IAuthService authService) : IRequestHandler<ApplicationUserRegisterCommand, ApiResponse<ApplicationUserRegisterResponseDto>>
{
    public async Task<ApiResponse<ApplicationUserRegisterResponseDto>> Handle(ApplicationUserRegisterCommand request, CancellationToken cancellationToken)
    {
        var (registerResponse, statusCode) = await authService.RegisterAsync(request.Model, cancellationToken: cancellationToken);
        string message = GetMessage(registerResponse, statusCode);
        return new ApiResponse<ApplicationUserRegisterResponseDto>(statusCode, message, registerResponse);
    }

    public static string GetMessage(ApplicationUserRegisterResponseDto? registerResponse, HttpStatusCode? statusCode) =>
        registerResponse is null || statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.NoContent ?
        $"Error occurred during creation of new user!" :
        statusCode switch
        {
            HttpStatusCode.Found => $"User by the username '{registerResponse.Username}' and email '{registerResponse.Email}' already exists!",
            HttpStatusCode.Created => $"User by the username '{registerResponse.Username}' and email '{registerResponse.Email}' has been successfully created!"
        };
}