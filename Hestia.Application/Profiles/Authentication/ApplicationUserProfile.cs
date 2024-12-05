using AutoMapper;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Domain.Models.Authentication;

namespace Hestia.Application.Profiles.Authentication;

public class ApplicationUserProfile : Profile
{
    public ApplicationUserProfile()
    {
        CreateMap<ApplicationUser, ApplicationUserRegisterResponseDto>();
        CreateMap<ApplicationUser, ApplicationUserLoginResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore());
    }
}