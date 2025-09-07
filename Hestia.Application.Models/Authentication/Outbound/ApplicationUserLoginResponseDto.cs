namespace Hestia.Application.Models.Authentication.Outbound;

public sealed record ApplicationUserLoginResponseDto
{
    public required string Username { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public required string Token { get; set; } = null!;
}