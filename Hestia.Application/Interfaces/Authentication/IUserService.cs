using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Domain.Models.Authentication;

namespace Hestia.Application.Interfaces.Authentication;

public interface IUserService
{
    Task<ApplicationUser?> CreateUserAsync(ApplicationUserRegisterDto model, CancellationToken cancellationToken = default);
}