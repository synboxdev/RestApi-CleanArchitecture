using AutoMapper;
using Hestia.Application.Profiles.Product;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;

namespace Hestia.Application.Tests.Profiles.Product;

public class ProductProfileTests
{
    private readonly IMapper _mapper;

    public ProductProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void ProductProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void UpdateProductDto_To_Product_Mapping()
    {
        // Arrange
        var updateProductDto = new UpdateProductDto
        {
            Id = Guid.NewGuid(),
            ExternalId = "ExternalId",
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            Price = 99.99M
        };
        var product = new Access.Entities.Product.Product(Guid.NewGuid())
        {
            ExternalId = "originalExternalId",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            Price = 99.99M,
            UserId = "UserId"
        };

        // Act
        var mappedProduct = _mapper.Map(updateProductDto, product);

        // Assert
        Assert.Equal(product.Id, mappedProduct.Id);
        Assert.Equal(product.ExternalId, mappedProduct.ExternalId);
        Assert.Equal(product.DateCreated, mappedProduct.DateCreated);
        Assert.Equal(updateProductDto.Name, mappedProduct.Name);
        Assert.Equal(updateProductDto.Description, mappedProduct.Description);
        Assert.Equal(updateProductDto.Price, mappedProduct.Price);
    }

    [Fact]
    public void Product_To_GetProductResponseDto_Mapping()
    {
        // Arrange
        var product = new Access.Entities.Product.Product(Guid.NewGuid())
        {
            ExternalId = "originalExternalId",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateEdited = DateTime.UtcNow.AddDays(-1),
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            Price = 99.99M,
            UserId = "UserId"
        };

        // Act
        var productResponseDto = _mapper.Map<GetProductResponseDto>(product);

        // Assert
        Assert.Equal(product.Id, productResponseDto.Id);
        Assert.Equal(product.ExternalId, productResponseDto.ExternalId);
        Assert.Equal(product.Name, productResponseDto.Name);
        Assert.Equal(product.Description, productResponseDto.Description);
        Assert.Equal(product.Price, productResponseDto.Price);
    }
}