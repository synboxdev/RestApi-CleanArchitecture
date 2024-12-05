namespace Hestia.Application.Models.Authentication.Inbound;

public class ApplicationUserLoginDto
{
    public required string Username { get; set; } = null!;
    public required string Password { get; set; } = null!;
    public required string Email { get; set; } = null!;
}