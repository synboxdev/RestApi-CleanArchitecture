using Hestia.Access.Entities.Product;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;

namespace Hestia.Application.Mappers;

public static partial class Mapper
{
    public static void ApplyUpdate(this Product product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.DateEdited = DateTime.UtcNow;
    }

    public static GetProductResponseDto ToResponseDto(this Product product)
    {
        return new GetProductResponseDto
        {
            Id = product.Id,
            ExternalId = product.ExternalId,
            DateCreated = product.DateCreated,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DateEdited = product.DateEdited
        };
    }
}