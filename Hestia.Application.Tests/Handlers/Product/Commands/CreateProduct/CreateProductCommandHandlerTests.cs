using Hestia.Application.Handlers.Product.Commands.CreateProduct;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Product;
using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Handlers.Product.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockTokenService = new Mock<ITokenService>();
        _handler = new CreateProductCommandHandler(_mockProductService.Object, _mockTokenService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenProductIsCreatedSuccessfully()
    {
        // Arrange
        var createProductDto = new CreateProductDto { ExternalId = "externalId", Name = "TestProduct", Description = "TestDescription", Price = 100.0M };
        string token = "sampleToken";
        string userId = "userId";
        var productId = Guid.NewGuid();
        _mockTokenService.Setup(x => x.GetIdentityUserIdByToken(token, It.IsAny<CancellationToken>())).ReturnsAsync(userId);
        _mockProductService.Setup(x => x.CreateProductAsync(createProductDto, It.IsAny<CancellationToken>())).ReturnsAsync((productId, HttpStatusCode.Created));

        var command = new CreateProductCommand(createProductDto, token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("Product was successfully created!", result.Message);
        Assert.Equal(productId, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnFound_WhenProductAlreadyExists()
    {
        // Arrange
        var createProductDto = new CreateProductDto { ExternalId = "externalId", Name = "TestProduct", Description = "TestDescription", Price = 100.0M };
        string token = "sampleToken";
        string userId = "userId";
        var productId = Guid.NewGuid();
        _mockTokenService.Setup(x => x.GetIdentityUserIdByToken(token, It.IsAny<CancellationToken>())).ReturnsAsync(userId);
        _mockProductService.Setup(x => x.CreateProductAsync(createProductDto, It.IsAny<CancellationToken>())).ReturnsAsync((productId, HttpStatusCode.Found));

        var command = new CreateProductCommand(createProductDto, token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.Found, result.StatusCode);
        Assert.Equal($"Product with ExternalId = 'externalId' already exists!", result.Message);
        Assert.Equal(productId, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenProductIsInvalid()
    {
        // Arrange
        var createProductDto = new CreateProductDto { ExternalId = "externalId", Name = "TestProduct", Description = "TestDescription", Price = 100.0M };
        string token = "sampleToken";
        string userId = "userId";
        _mockTokenService.Setup(x => x.GetIdentityUserIdByToken(token, It.IsAny<CancellationToken>())).ReturnsAsync(userId);
        _mockProductService.Setup(x => x.CreateProductAsync(createProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, HttpStatusCode.BadRequest));

        var command = new CreateProductCommand(createProductDto, token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Product couldn't be created due to invalid values!", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var createProductDto = new CreateProductDto { ExternalId = "externalId", Name = "TestProduct", Description = "TestDescription", Price = 100.0M };
        string token = "sampleToken";
        string userId = "userId";
        _mockTokenService.Setup(x => x.GetIdentityUserIdByToken(token, It.IsAny<CancellationToken>())).ReturnsAsync(userId);
        _mockProductService.Setup(x => x.CreateProductAsync(It.IsAny<CreateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, HttpStatusCode.InternalServerError));

        var command = new CreateProductCommand(createProductDto, token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("Error occurred during product creation!", result.Message);
        Assert.Null(result.Data);
    }
}