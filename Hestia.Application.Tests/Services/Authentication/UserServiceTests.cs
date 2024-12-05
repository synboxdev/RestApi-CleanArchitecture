using Hestia.Access.Requests.User.Commands.CreateUser;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Services.Authentication;
using Hestia.Domain.Enumerations;
using Hestia.Domain.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Hestia.Application.Tests.Services.Authentication;

public class UserServiceTests
{
    private readonly Mock<IAccessLayer> _mockAccessLayer;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockAccessLayer = new Mock<IAccessLayer>();
        _userService = new UserService(_mockAccessLayer.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnApplicationUser_WhenUserCreationIsSuccessful()
    {
        // Arrange
        var model = new ApplicationUserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User",
            Role = Role.User
        };

        var applicationUser = new ApplicationUser
        {
            Id = "1",
            UserName = model.Username,
            Email = model.Email,
            Role = model.Role
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateApplicationUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateInternalUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.CreateUserAsync(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(applicationUser.UserName, result.UserName);
        Assert.Equal(applicationUser.Email, result.Email);
        Assert.Equal(applicationUser.Role, result.Role);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnNull_WhenApplicationUserCreationFails()
    {
        // Arrange
        var model = new ApplicationUserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User",
            Role = Role.User
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateApplicationUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _userService.CreateUserAsync(model);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnNull_WhenInternalUserCreationFails()
    {
        // Arrange
        var model = new ApplicationUserRegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User",
            Role = Role.User
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateApplicationUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateInternalUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.CreateUserAsync(model);

        // Assert
        Assert.Null(result);
    }
}