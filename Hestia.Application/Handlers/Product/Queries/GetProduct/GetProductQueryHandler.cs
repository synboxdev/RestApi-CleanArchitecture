using Hestia.Application.Interfaces.Product;
using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using MediatR;
using System.Net;

namespace Hestia.Application.Handlers.Product.Queries.GetProduct;

public class GetProductQueryHandler(IProductService productService) : IRequestHandler<GetProductQuery, ApiResponse<GetProductResponseDto?>>
{
    public async Task<ApiResponse<GetProductResponseDto?>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var (getProductResponse, statusCode) = await productService.GetProductAsync(request.Product, cancellationToken);
        string message = GetMessage(request, statusCode);
        return new ApiResponse<GetProductResponseDto?>(statusCode, message, getProductResponse);
    }

    public static string GetMessage(GetProductQuery request, HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.OK => $"Product was successfully found!",
            HttpStatusCode.NotFound => $"Product with Id = '{request.Product.Id}' and ExternalId = '{request.Product.ExternalId}' was NOT found!",
            HttpStatusCode.InternalServerError => $"Error occurred during product retrieval!"
        };
}