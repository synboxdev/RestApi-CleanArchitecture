using AutoMapper;
using Hestia.Access.Requests.Product.Commands.CreateProduct;
using Hestia.Access.Requests.Product.Commands.DeleteProduct;
using Hestia.Access.Requests.Product.Commands.UpdateProduct;
using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Application.Interfaces.Infrastructure;
using Hestia.Application.Interfaces.Product;
using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Hestia.Application.Services.Product;

public class ProductService(IAccessLayer accessLayer, IMapper mapper, ILogger<ProductService> logger) : IProductService
{
    public async Task<(Guid?, HttpStatusCode)> CreateProductAsync(CreateProductDto product, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingProduct = await GetExistingProductAsync(product.GetUserId()!, product.ExternalId, cancellationToken);

            if (existingProduct is not null)
                return (existingProduct.Id, HttpStatusCode.Found);

            var newProduct = CreateProduct(product);
            bool newProductCreateResult = await ExecuteCreateProductCommandAsync(newProduct, cancellationToken);

            return newProductCreateResult ? (newProduct.Id, HttpStatusCode.Created) : (null, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occurred during creation of product with ExternalId: {product.ExternalId}. Exception: {ex.Message}");
            return (null, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<(bool, HttpStatusCode)> UpdateProductAsync(UpdateProductDto product, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingProduct = await GetExistingProductAsync(product.Id, product.ExternalId, cancellationToken);

            if (existingProduct is null)
                return (false, HttpStatusCode.NotFound);

            existingProduct = mapper.Map(product, existingProduct);
            bool existingProductUpdateResult = await ExecuteUpdateProductCommandAsync(existingProduct, cancellationToken);

            return existingProductUpdateResult ? (true, HttpStatusCode.OK) : (false, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occurred during update of product with Id = '{product.Id}' and ExternalId = '{product.ExternalId}'. Exception: {ex.Message}");
            return (false, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<(bool, HttpStatusCode)> DeleteProductAsync(GetProductDto product, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingProduct = await GetExistingProductAsync(product, cancellationToken);

            if (existingProduct is null)
                return (false, HttpStatusCode.NotFound);

            bool productDeleteResult = await ExecuteDeleteProductCommandAsync(existingProduct, cancellationToken);

            return productDeleteResult ? (true, HttpStatusCode.OK) : (false, HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occurred during deletion of a product. Exception: {ex.Message}");
            return (false, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<(GetProductResponseDto?, HttpStatusCode)> GetProductAsync(GetProductDto product, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingProduct = await GetExistingProductAsync(product, cancellationToken);

            return existingProduct is null ?
                ((GetProductResponseDto?, HttpStatusCode))(null, HttpStatusCode.NotFound) :
                (mapper.Map<GetProductResponseDto>(existingProduct), HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occurred during retrieval of a product. Exception: {ex.Message}");
            return (null, HttpStatusCode.InternalServerError);
        }
    }


    private async Task<Access.Entities.Product.Product?> GetExistingProductAsync(string userId, string externalId, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new GetExistingProductByUserQuery(userId, externalId), cancellationToken);

    private async Task<Access.Entities.Product.Product?> GetExistingProductAsync(Guid id, string externalId, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new GetExistingProductByCompositeIdQuery(id, externalId), cancellationToken);

    private async Task<Access.Entities.Product.Product?> GetExistingProductAsync(Guid? id, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new GetExistingProductByIdQuery(id), cancellationToken);

    private async Task<Access.Entities.Product.Product?> GetExistingProductAsync(string externalId, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new GetExistingProductByExternalIdQuery(externalId), cancellationToken);

    /// <summary>
    /// Attempts to retrieve an existing product by either composite key of Id AND ExternalId, or by each of those fields individually. 
    /// Worst case - returns null if no such product is found.
    /// </summary>
    private async Task<Access.Entities.Product.Product?> GetExistingProductAsync(GetProductDto product, CancellationToken cancellationToken = default) =>
        product.Id.HasValue && product.Id != Guid.Empty && !string.IsNullOrEmpty(product.ExternalId) ?
        await GetExistingProductAsync((Guid)product.Id, product.ExternalId, cancellationToken) :
        !product.Id.HasValue || product.Id == Guid.Empty ?
        await GetExistingProductAsync(product.ExternalId, cancellationToken) :
        string.IsNullOrEmpty(product.ExternalId) ?
        await GetExistingProductAsync(product.Id, cancellationToken) : null;

    private async Task<bool> ExecuteCreateProductCommandAsync(Access.Entities.Product.Product product, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new CreateProductCommand(product), cancellationToken);

    private async Task<bool> ExecuteUpdateProductCommandAsync(Access.Entities.Product.Product product, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new UpdateProductCommand(product), cancellationToken);

    private async Task<bool> ExecuteDeleteProductCommandAsync(Access.Entities.Product.Product product, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new DeleteProductCommand(product), cancellationToken);

    private static Access.Entities.Product.Product CreateProduct(CreateProductDto productDto) =>
        new(Guid.NewGuid())
        {
            UserId = productDto.GetUserId()!,
            ExternalId = productDto.ExternalId,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            DateCreated = DateTime.UtcNow,
        };
}