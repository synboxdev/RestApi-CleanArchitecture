using Hestia.Application.Interfaces.Product;
using Hestia.Application.Models.Shared;
using MediatR;
using System.Net;

namespace Hestia.Application.Handlers.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IProductService productService) : IRequestHandler<UpdateProductCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var (updateProductResponse, statusCode) = await productService.UpdateProductAsync(request.Product, cancellationToken);
        string message = GetMessage(request, statusCode);
        return new ApiResponse<bool>(statusCode, message, updateProductResponse);
    }

    public static string GetMessage(UpdateProductCommand request, HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.OK => $"Product was successfully updated!",
            HttpStatusCode.BadRequest => $"Product couldn't be updated due to invalid values!",
            HttpStatusCode.NotFound => $"Product with Id = '{request.Product.Id}' and ExternalId = '{request.Product.ExternalId}' was NOT found!",
            HttpStatusCode.InternalServerError => $"Error occurred during product update!"
        };
}