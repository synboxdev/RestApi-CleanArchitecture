using Hestia.Application.Interfaces.Product;
using Hestia.Application.Models.Shared;
using MediatR;
using System.Net;

namespace Hestia.Application.Handlers.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IProductService productService) : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var (updateProductResponse, statusCode) = await productService.DeleteProductAsync(request.Product, cancellationToken);
        string message = GetMessage(updateProductResponse, request, statusCode);
        return new ApiResponse<bool>(statusCode, message, updateProductResponse);
    }

    public static string GetMessage(bool? deleteProductResponse, DeleteProductCommand request, HttpStatusCode statusCode) =>
        deleteProductResponse is null || statusCode == HttpStatusCode.InternalServerError ? $"Error occurred during product update!" :
        statusCode switch
        {
            HttpStatusCode.OK => $"Product was successfully deleted!",
            HttpStatusCode.NotFound => $"Product with Id = '{request.Product.Id}' and ExternalId = '{request.Product.ExternalId}' was NOT found!"
        };
}