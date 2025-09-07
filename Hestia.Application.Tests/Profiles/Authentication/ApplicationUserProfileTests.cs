using Hestia.Application.Mappers;
using Hestia.Domain.Enumerations;
using Hestia.Domain.Models.Authentication;

namespace Hestia.Application.Tests.Profiles.Authentication;

public class ApplicationUserProfileTests
{
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
        var registerResponseDto = applicationUser.ToRegisterResponseDto();

        // Assert
        Assert.Equal(applicationUser.UserName, registerResponseDto.Username);
        Assert.Equal(applicationUser.Email, registerResponseDto.Email);
    }
}