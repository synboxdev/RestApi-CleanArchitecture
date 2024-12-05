using AutoMapper;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Profiles.Authentication;
using Hestia.Domain.Enumerations;
using Hestia.Domain.Models.Authentication;

namespace Hestia.Application.Tests.Profiles.Authentication;

public class ApplicationUserProfileTests
{
    private readonly IMapper _mapper;

    public ApplicationUserProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ApplicationUserProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void ApplicationUserProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ApplicationUserProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void ApplicationUser_To_ApplicationUserRegisterResponseDto_Mapping()
    {
        // Arrange
        var applicationUser = new ApplicationUser
        {
            Id = "1",
            UserName = "testuser",
            Email = "test@example.com",
            Role = Role.User
        };

        // Act
        var registerResponseDto = _mapper.Map<ApplicationUserRegisterResponseDto>(applicationUser);

        // Assert
        Assert.Equal(applicationUser.UserName, registerResponseDto.Username);
        Assert.Equal(applicationUser.Email, registerResponseDto.Email);
    }

    [Fact]
    public void ApplicationUser_To_ApplicationUserLoginResponseDto_Mapping()
    {
        // Arrange
        var applicationUser = new ApplicationUser
        {
            Id = "1",
            UserName = "testuser",
            Email = "test@example.com",
            Role = Role.User
        };

        // Act
        var loginResponseDto = _mapper.Map<ApplicationUserLoginResponseDto>(applicationUser);

        // Assert
        Assert.Equal(applicationUser.UserName, loginResponseDto.Username);
        Assert.Equal(applicationUser.Email, loginResponseDto.Email);
    }
}