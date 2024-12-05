using AutoMapper;
using Hestia.Access.Requests.Product.Commands.CreateProduct;
using Hestia.Access.Requests.Product.Commands.DeleteProduct;
using Hestia.Access.Requests.Product.Commands.UpdateProduct;
using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Services.Product;
using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Hestia.Application.Tests.Services.Product;

public class ProductServiceCreateProductTests
{
    private readonly Mock<IAccessLayer> _mockAccessLayer;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _productService;

    public ProductServiceCreateProductTests()
    {
        _mockAccessLayer = new Mock<IAccessLayer>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_mockAccessLayer.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnCreated_WhenProductIsCreatedSuccessfully()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            ExternalId = "externalId",
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100.0M
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Access.Entities.Product.Product)null);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var (productId, statusCode) = await _productService.CreateProductAsync(createProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, statusCode);
        Assert.True(productId.HasValue);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnFound_WhenProductAlreadyExists()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            ExternalId = "externalId",
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100.0M
        };
        var existingProduct = new Access.Entities.Product.Product(Guid.NewGuid())
        {
            UserId = "UserId",
            DateCreated = DateTime.UtcNow,
            ExternalId = createProductDto.ExternalId,
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var (productId, statusCode) = await _productService.CreateProductAsync(createProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.Found, statusCode);
        Assert.Equal(existingProduct.Id, productId);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            ExternalId = "externalId",
            Name = "TestProduct",
            Description = "TestDescription",
            Price = 100.0M
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByUserQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (productId, statusCode) = await _productService.CreateProductAsync(createProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
        Assert.Null(productId);
    }


    [Fact]
    public async Task UpdateProductAsync_ShouldReturnOk_WhenProductIsUpdatedSuccessfully()
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
        var existingProduct = new Access.Entities.Product.Product(updateProductDto.Id)
        {
            UserId = "UserId",
            DateCreated = DateTime.UtcNow,
            ExternalId = updateProductDto.ExternalId,
            Name = updateProductDto.Name,
            Description = updateProductDto.Description,
            Price = updateProductDto.Price
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockMapper.Setup(m => m.Map(updateProductDto, existingProduct)).Returns(existingProduct);

        // Act
        var (isUpdated, statusCode) = await _productService.UpdateProductAsync(updateProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.True(isUpdated);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
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

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Access.Entities.Product.Product)null);

        // Act
        var (isUpdated, statusCode) = await _productService.UpdateProductAsync(updateProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
        Assert.False(isUpdated);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
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

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (isUpdated, statusCode) = await _productService.UpdateProductAsync(updateProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
        Assert.False(isUpdated);
    }


    [Fact]
    public async Task DeleteProductAsync_ShouldReturnOk_WhenProductIsDeletedSuccessfully()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };
        var existingProduct = new Access.Entities.Product.Product(getProductDto.Id.Value)
        {
            ExternalId = getProductDto.ExternalId,
            UserId = "UserId",
            DateCreated = DateTime.UtcNow,
            Name = "name",
            Description = "desc",
            Price = 123M
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var (isDeleted, statusCode) = await _productService.DeleteProductAsync(getProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.True(isDeleted);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Access.Entities.Product.Product)null);

        // Act
        var (isDeleted, statusCode) = await _productService.DeleteProductAsync(getProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
        Assert.False(isDeleted);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (isDeleted, statusCode) = await _productService.DeleteProductAsync(getProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
        Assert.False(isDeleted);
    }


    [Fact]
    public async Task GetProductAsync_ShouldReturnOk_WhenProductIsFound()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };
        var existingProduct = new Access.Entities.Product.Product(getProductDto.Id.Value)
        {
            ExternalId = getProductDto.ExternalId,
            UserId = "UserId",
            DateCreated = DateTime.UtcNow,
            Name = "name",
            Description = "Description",
            Price = 123M
        };
        var getProductResponseDto = new GetProductResponseDto
        {
            Id = existingProduct.Id,
            ExternalId = existingProduct.ExternalId,
            Name = existingProduct.Name,
            Description = existingProduct.Description,
            Price = existingProduct.Price
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _mockMapper.Setup(m => m.Map<GetProductResponseDto>(existingProduct)).Returns(getProductResponseDto);

        // Act
        var (productResponse, statusCode) = await _productService.GetProductAsync(getProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(productResponse);
        Assert.Equal(existingProduct.Id, productResponse.Id);
    }

    [Fact]
    public async Task GetProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Access.Entities.Product.Product)null);

        // Act
        var (productResponse, statusCode) = await _productService.GetProductAsync(getProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, statusCode);
        Assert.Null(productResponse);
    }

    [Fact]
    public async Task GetProductAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var getProductDto = new GetProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "externalId"
        };

        _mockAccessLayer.Setup(x => x.ExecuteAsync(It.IsAny<GetExistingProductByCompositeIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (productResponse, statusCode) = await _productService.GetProductAsync(getProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
        Assert.Null(productResponse);
    }
}