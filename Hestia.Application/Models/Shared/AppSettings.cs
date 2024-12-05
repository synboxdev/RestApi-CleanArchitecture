namespace Hestia.Application.Models.Shared;

public record Jwt
{
    public string ValidAudience { get; init; } = null!;
    public string ValidIssuer { get; init; } = null!;
    public int TokenExpiryInHours { get; init; } = 1;
    public string Secret { get; init; } = null!;
}