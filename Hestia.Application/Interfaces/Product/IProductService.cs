using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;
using System.Net;

namespace Hestia.Application.Interfaces.Product;

public interface IProductService
{
    Task<(Guid?, HttpStatusCode)> CreateProductAsync(CreateProductDto Product, CancellationToken cancellationToken = default);
    Task<(bool, HttpStatusCode)> UpdateProductAsync(UpdateProductDto Product, CancellationToken cancellationToken = default);
    Task<(bool, HttpStatusCode)> DeleteProductAsync(GetProductDto Product, CancellationToken cancellationToken = default);
    Task<(GetProductResponseDto?, HttpStatusCode)> GetProductAsync(GetProductDto Product, CancellationToken cancellationToken = default);
}