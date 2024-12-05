using Hestia.Domain.Enumerations;

namespace Hestia.Application.Models.Authentication.Outbound;

public sealed record ApplicationUserRegisterResponseDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public Role Role { get; set; }
}