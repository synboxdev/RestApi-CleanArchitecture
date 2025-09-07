using Hestia.Application.Mappers;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;

namespace Hestia.Application.Tests.Profiles.Product;

public class ProductProfileTests
{
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

        var product = new Access.Entities.Product.Product(updateProductDto.Id)
        {
            ExternalId = updateProductDto.ExternalId,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            Name = "OriginalName",
            Description = "OriginalDescription",
            Price = 10.00M,
            UserId = "UserId"
        };

        // Act
        product.ApplyUpdate(updateProductDto);

        // Assert
        Assert.Equal(updateProductDto.Id, product.Id);
        Assert.Equal(updateProductDto.ExternalId, product.ExternalId);
        Assert.Equal(updateProductDto.Name, product.Name);
        Assert.Equal(updateProductDto.Description, product.Description);
        Assert.Equal(updateProductDto.Price, product.Price);
        Assert.True(product.DateEdited > product.DateCreated);
    }

    [Fact]
    public void Product_To_GetProductResponseDto_Mapping()
    {
        // Arrange
        var product = new Access.Entities.Product.Product(Guid.NewGuid())
        {
            ExternalId = "originalExternalId",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateEdited = DateTime.UtcNow,
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            Price = 99.99M,
            UserId = "UserId"
        };

        // Act
        var productResponseDto = product.ToResponseDto();

        // Assert
        Assert.Equal(product.Id, productResponseDto.Id);
        Assert.Equal(product.ExternalId, productResponseDto.ExternalId);
        Assert.Equal(product.DateCreated, productResponseDto.DateCreated);
        Assert.Equal(product.DateEdited, productResponseDto.DateEdited);
        Assert.Equal(product.Name, productResponseDto.Name);
        Assert.Equal(product.Description, productResponseDto.Description);
        Assert.Equal(product.Price, productResponseDto.Price);
    }
}
