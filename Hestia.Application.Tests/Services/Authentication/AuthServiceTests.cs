using AutoMapper;
using Hestia.Access.Requests.User.Queries.UserExists;
using Hestia.Access.Requests.User.Queries.ValidateUserLogin;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Hestia.Application.Services.Authentication;
using Hestia.Domain.Models.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Services.Authentication;

public class AuthServiceTests
{
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IAccessLayer> _mockAccessLayer;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockTokenService = new Mock<ITokenService>();
        _mockUserService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockAccessLayer = new Mock<IAccessLayer>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(
            _mockTokenService.Object,
            _mockUserService.Object,
            _mockMapper.Object,
            _mockAccessLayer.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnOk_WhenUserIsValid()
    {
        // Arrange
        var model = new ApplicationUserLoginDto { Username = "testuser", Email = "test@example.com", Password = "password123" };
        var existingUser = new ApplicationUser { UserName = "testuser", Email = "test@example.com" };
        string token = "sampleToken";

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<ValidateUserLoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockTokenService.Setup(x => x.GenerateTokenAsync(existingUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act
        var (response, statusCode) = await _authService.LoginAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.Equal("testuser", response.Username);
        Assert.Equal("test@example.com", response.Email);
        Assert.Equal(token, response.Token);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var model = new ApplicationUserLoginDto { Username = "nonexistent", Email = "nonexistent@example.com", Password = "password123" };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var (response, statusCode) = await _authService.LoginAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
        Assert.Null(response.Token);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        var model = new ApplicationUserLoginDto { Username = "testuser", Email = "test@example.com", Password = "wrongpassword" };
        var existingUser = new ApplicationUser { UserName = "testuser", Email = "test@example.com" };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<ValidateUserLoginQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var (response, statusCode) = await _authService.LoginAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.Null(response.Token);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnCreated_WhenUserIsRegisteredSuccessfully()
    {
        // Arrange
        var model = new ApplicationUserRegisterDto { Username = "newuser", Name = "name", Email = "newuser@example.com", Password = "password123" };
        var newUser = new ApplicationUser { UserName = "newuser", Email = "newuser@example.com" };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser)null);

        _mockUserService.Setup(x => x.CreateUserAsync(model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newUser);

        _mockMapper.Setup(m => m.Map<ApplicationUserRegisterResponseDto>(It.IsAny<ApplicationUser>()))
            .Returns(new ApplicationUserRegisterResponseDto { Username = "newuser", Email = "newuser@example.com" });

        // Act
        var (response, statusCode) = await _authService.RegisterAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Created, statusCode);
        Assert.Equal("newuser", response.Username);
        Assert.Equal("newuser@example.com", response.Email);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFound_WhenUserAlreadyExists()
    {
        // Arrange
        var model = new ApplicationUserRegisterDto { Username = "newuser", Name = "name", Email = "newuser@example.com", Password = "password123" };
        var existingUser = new ApplicationUser { UserName = "existinguser", Email = "existinguser@example.com" };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var (response, statusCode) = await _authService.RegisterAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.Found, statusCode);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var model = new ApplicationUserRegisterDto { Username = "newuser", Name = "name", Email = "newuser@example.com", Password = "password123" };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (response, statusCode) = await _authService.RegisterAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var model = new ApplicationUserLoginDto { Username = "testuser", Email = "test@example.com", Password = "password123" };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingUserQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (response, statusCode) = await _authService.LoginAsync(model);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
    }
}