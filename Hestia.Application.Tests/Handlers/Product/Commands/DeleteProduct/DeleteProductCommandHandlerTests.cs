using Hestia.Application.Handlers.Product.Commands.DeleteProduct;
using Hestia.Application.Interfaces.Product;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Handlers.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _handler = new DeleteProductCommandHandler(_mockProductService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenProductIsDeletedSuccessfully()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };
        _mockProductService.Setup(x => x.DeleteProductAsync(getProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, HttpStatusCode.OK));

        var command = new DeleteProductCommand(getProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Product was successfully deleted!", result.Message);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };
        _mockProductService.Setup(x => x.DeleteProductAsync(getProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, HttpStatusCode.NotFound));

        var command = new DeleteProductCommand(getProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal($"Product with Id = '{getProductDto.Id}' and ExternalId = '{getProductDto.ExternalId}' was NOT found!", result.Message);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };
        _mockProductService.Setup(x => x.DeleteProductAsync(It.IsAny<GetProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, HttpStatusCode.InternalServerError));

        var command = new DeleteProductCommand(getProductDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("Error occurred during product update!", result.Message);
        Assert.False(result.Data);
    }
}
