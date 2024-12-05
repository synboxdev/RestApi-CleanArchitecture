using Hestia.Application.Handlers.Authentication.Commands.Login;
using Hestia.Application.Handlers.Authentication.Commands.Register;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Hestia.Api.Controllers;

/// <summary>
/// Authentication endpoints allow for registration and login of authenticated users, which is required to access other parts of the system
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(ICoreLayer coreLayer) : ControllerBase
{
    /// <summary>
    /// Provides an authentication token, which is needed for other endpoints
    /// </summary>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationUserLoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(ApplicationUserLoginDto model, CancellationToken cancellationToken = default)
    {
        var result = await coreLayer.ExecuteAsync(new ApplicationUserLoginCommand(model), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Registers an authentication user, which can then be used to login and receive an authentication token
    /// </summary>
    [HttpPost("Register")]
    [ProducesResponseType(typeof(ApiResponse<ApplicationUserRegisterResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(ApplicationUserRegisterDto model, CancellationToken cancellationToken = default)
    {
        var result = await coreLayer.ExecuteAsync(new ApplicationUserRegisterCommand(model), cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }
}