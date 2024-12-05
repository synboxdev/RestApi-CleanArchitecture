using Hestia.Application.Handlers.Product.Commands.UpdateProduct;
using Hestia.Application.Interfaces.Product;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Handlers.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _handler = new UpdateProductCommandHandler(_mockProductService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenProductIsUpdatedSuccessfully()
    {
        // Arrange
        var updateProductDto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId",
            Name = "UpdatedProduct",
            Description = "UpdatedDescription",
            Price = 200.0M
        };
        _mockProductService.Setup(x => x.UpdateProductAsync(updateProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, HttpStatusCode.OK));

        var command = new UpdateProductCommand(updateProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Product was successfully updated!", result.Message);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenProductIsInvalid()
    {
        // Arrange
        var updateProductDto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId",
            Name = "UpdatedProduct",
            Description = "UpdatedDescription",
            Price = 200.0M
        };
        _mockProductService.Setup(x => x.UpdateProductAsync(updateProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, HttpStatusCode.BadRequest));

        var command = new UpdateProductCommand(updateProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Product couldn't be updated due to invalid values!", result.Message);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var updateProductDto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId",
            Name = "UpdatedProduct",
            Description = "UpdatedDescription",
            Price = 200.0M
        };
        _mockProductService.Setup(x => x.UpdateProductAsync(updateProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, HttpStatusCode.NotFound));

        var command = new UpdateProductCommand(updateProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal($"Product with Id = '{updateProductDto.Id}' and ExternalId = '{updateProductDto.ExternalId}' was NOT found!", result.Message);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var updateProductDto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId",
            Name = "UpdatedProduct",
            Description = "UpdatedDescription",
            Price = 200.0M
        };
        _mockProductService.Setup(x => x.UpdateProductAsync(It.IsAny<UpdateProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, HttpStatusCode.InternalServerError));

        var command = new UpdateProductCommand(updateProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("Error occurred during product update!", result.Message);
        Assert.False(result.Data);
    }
}