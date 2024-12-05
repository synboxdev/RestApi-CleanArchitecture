using Hestia.Application.Handlers.Authentication.Commands.Login;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Handlers.Authentication.Commands.Login;

public class ApplicationUserLoginCommandHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly ApplicationUserLoginCommandHandler _handler;

    public ApplicationUserLoginCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _handler = new ApplicationUserLoginCommandHandler(_mockAuthService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginModel = new ApplicationUserLoginDto { Username = "testuser", Password = "password123", Email = "test@example.com" };
        var loginResponse = new ApplicationUserLoginResponseDto { Username = "testuser", Email = "test@example.com", Token = "sampleToken" };
        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<ApplicationUserLoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((loginResponse, HttpStatusCode.OK));

        var command = new ApplicationUserLoginCommand(loginModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Login for the user 'testuser' was successful!", result.Message);
        Assert.Equal(loginResponse, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var loginModel = new ApplicationUserLoginDto { Username = "testuser", Password = "password123", Email = "test@example.com" };
        var loginResponse = new ApplicationUserLoginResponseDto { Username = "testuser", Email = "test@example.com", Token = "sampleToken" };
        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<ApplicationUserLoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((loginResponse, HttpStatusCode.NotFound));

        var command = new ApplicationUserLoginCommand(loginModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("User by the username 'testuser' does not exist! Make sure that provided credentials are correct!", result.Message);
        Assert.Equal(loginResponse, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        var loginModel = new ApplicationUserLoginDto { Username = "testuser", Password = "password123", Email = "test@example.com" };
        var loginResponse = new ApplicationUserLoginResponseDto { Username = "testuser", Email = "test@example.com", Token = "sampleToken" };
        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<ApplicationUserLoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((loginResponse, HttpStatusCode.BadRequest));

        var command = new ApplicationUserLoginCommand(loginModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Invalid password provided for user 'testuser'!", result.Message);
        Assert.Equal(loginResponse, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var loginModel = new ApplicationUserLoginDto { Username = "testuser", Password = "password123", Email = "test@example.com" };
        _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<ApplicationUserLoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, HttpStatusCode.InternalServerError));

        var command = new ApplicationUserLoginCommand(loginModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("Error occurred during login!", result.Message);
        Assert.Null(result.Data);
    }
}