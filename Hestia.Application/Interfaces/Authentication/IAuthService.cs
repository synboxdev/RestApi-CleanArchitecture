using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Domain.Enumerations;
using System.Net;

namespace Hestia.Application.Interfaces.Authentication;

public interface IAuthService
{
    Task<(ApplicationUserRegisterResponseDto, HttpStatusCode)> RegisterAsync(ApplicationUserRegisterDto model, Role? role = null, CancellationToken cancellationToken = default);
    Task<(ApplicationUserLoginResponseDto, HttpStatusCode)> LoginAsync(ApplicationUserLoginDto model, CancellationToken cancellationToken = default);
}