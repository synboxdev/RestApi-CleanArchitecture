using Hestia.Domain.Models.Authentication;

namespace Hestia.Application.Interfaces.Authentication;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<string?> GetIdentityUserIdByToken(string token, CancellationToken cancellationToken = default);
}