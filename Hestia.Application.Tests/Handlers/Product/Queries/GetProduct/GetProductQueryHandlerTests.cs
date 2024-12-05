using Hestia.Application.Handlers.Product.Queries.GetProduct;
using Hestia.Application.Interfaces.Product;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Handlers.Product.Queries.GetProduct;

public class GetProductQueryHandlerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly GetProductQueryHandler _handler;

    public GetProductQueryHandlerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _handler = new GetProductQueryHandler(_mockProductService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenProductIsFound()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };
        var productResponse = new GetProductResponseDto
        {
            Id = getProductDto.Id.Value,
            ExternalId = getProductDto.ExternalId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 100.0M
        };
        _mockProductService.Setup(x => x.GetProductAsync(getProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((productResponse, HttpStatusCode.OK));

        var query = new GetProductQuery(getProductDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Product was successfully found!", result.Message);
        Assert.Equal(productResponse, result.Data);
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
        _mockProductService.Setup(x => x.GetProductAsync(getProductDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, HttpStatusCode.NotFound));

        var query = new GetProductQuery(getProductDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal($"Product with Id = '{getProductDto.Id}' and ExternalId = '{getProductDto.ExternalId}' was NOT found!", result.Message);
        Assert.Null(result.Data);
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
        _mockProductService.Setup(x => x.GetProductAsync(It.IsAny<GetProductDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, HttpStatusCode.InternalServerError));

        var query = new GetProductQuery(getProductDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Equal("Error occurred during product retrieval!", result.Message);
        Assert.Null(result.Data);
    }
}