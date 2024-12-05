using Hestia.Domain.Enumerations;

namespace Hestia.Application.Models.Authentication.Inbound;

public class ApplicationUserRegisterDto
{
    public required string Name { get; set; } = null!;
    public required string Username { get; set; } = null!;
    public required string Password { get; set; } = null!;
    public required string Email { get; set; } = null!;
    public Role Role { get; set; } = Role.User;
}