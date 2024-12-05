using Hestia.Application.Handlers.Authentication.Commands.Register;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Application.Models.Authentication.Outbound;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Handlers.Authentication.Commands.Register;

public class ApplicationUserRegisterCommandHandlerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly ApplicationUserRegisterCommandHandler _handler;

    public ApplicationUserRegisterCommandHandlerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _handler = new ApplicationUserRegisterCommandHandler(_mockAuthService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenRegisterIsSuccessful()
    {
        // Arrange
        var registerModel = new ApplicationUserRegisterDto { Username = "testuser", Email = "test@example.com", Password = "password123", Name = "name" };
        var registerResponse = new ApplicationUserRegisterResponseDto { Username = "testuser", Email = "test@example.com" };
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<ApplicationUserRegisterDto>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((registerResponse, HttpStatusCode.Created));

        var command = new ApplicationUserRegisterCommand(registerModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("User by the username 'testuser' and email 'test@example.com' has been successfully created!", result.Message);
        Assert.Equal(registerResponse, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnFound_WhenUserAlreadyExists()
    {
        // Arrange
        var registerModel = new ApplicationUserRegisterDto { Username = "testuser", Email = "test@example.com", Password = "password123", Name = "name" };
        var registerResponse = new ApplicationUserRegisterResponseDto { Username = "testuser", Email = "test@example.com" };
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<ApplicationUserRegisterDto>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((registerResponse, HttpStatusCode.Found));

        var command = new ApplicationUserRegisterCommand(registerModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.Found, result.StatusCode);
        Assert.Equal("User by the username 'testuser' and email 'test@example.com' already exists!", result.Message);
        Assert.Equal(registerResponse, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var registerModel = new ApplicationUserRegisterDto { Username = "testuser", Email = "test@example.com", Password = "password123", Name = "name" };
        _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<ApplicationUserRegisterDto>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, HttpStatusCode.InternalServerError));

        var command = new ApplicationUserRegisterCommand(registerModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("Error occurred during creation of new user!", result.Message);
        Assert.Null(result.Data);
    }
}