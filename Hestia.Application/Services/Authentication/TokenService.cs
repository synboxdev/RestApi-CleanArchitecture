using Hestia.Access.Requests.Authentication.Commands.CreateTokenLog;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Layers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hestia.Application.Services.Authentication;

public class TokenService(IAccessLayer accessLayer, IMemoryCache memoryCache, UserManager<ApplicationUser> userManager, IOptions<Jwt> jwt) : ITokenService
{
    private readonly IOptions<Jwt> jwt = jwt;

    public async Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenExpirationDate = DateTime.UtcNow.AddHours(jwt.Value.TokenExpiryInHours);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
                claims: claims,
                expires: tokenExpirationDate,
                audience: jwt.Value.ValidAudience,
                issuer: jwt.Value.ValidIssuer,
                signingCredentials: credentials
            );

        string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        await accessLayer.ExecuteAsync(new CreateOrUpdateTokenLogCommand(serializedToken, user.Id, tokenExpirationDate), cancellationToken);
        memoryCache.Set(serializedToken, user.Id);

        return serializedToken;
    }

    public async Task<string?> GetIdentityUserIdByToken(string token, CancellationToken cancellationToken = default) =>
        !string.IsNullOrEmpty(token) && memoryCache.TryGetValue(token, out string? identityUserId) && await userManager.FindByIdAsync(identityUserId!) is not null ?
        identityUserId : null;
}