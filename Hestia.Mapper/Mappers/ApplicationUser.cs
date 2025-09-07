using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Domain.Models.Authentication;

namespace Hestia.Application.Mappers;

public static partial class Mapper
{
    public static ApplicationUserRegisterResponseDto ToRegisterResponseDto(this ApplicationUser user)
    {
        return new ApplicationUserRegisterResponseDto
        {
            Username = user.UserName,
            Email = user.Email,
            Role = user.Role
        };
    }
}