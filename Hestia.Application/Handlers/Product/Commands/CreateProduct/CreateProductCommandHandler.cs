using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Interfaces.Product;
using Hestia.Application.Models.Shared;
using Hestia.Mediator.Infrastructure.Messaging;
using System.Net;

namespace Hestia.Application.Handlers.Product.Commands.CreateProduct;

public class CreateProductCommandHandler(IProductService productService, ITokenService tokenService) : IRequestHandler<CreateProductCommand, ApiResponse<Guid?>>
{
    public async Task<ApiResponse<Guid?>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        string? userId = await GetIdentityUserIdFromCache(request.Token, cancellationToken);
        request.Product.SetUserId(userId!);
        var (createProductResponse, statusCode) = await productService.CreateProductAsync(request.Product, cancellationToken);
        string message = GetMessage(request, statusCode);
        return new ApiResponse<Guid?>(statusCode, message, createProductResponse);
    }

    public static string GetMessage(CreateProductCommand request, HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.Created => $"Product was successfully created!",
            HttpStatusCode.Found => $"Product with ExternalId = '{request.Product.ExternalId}' already exists!",
            HttpStatusCode.BadRequest => $"Product couldn't be created due to invalid values!",
            HttpStatusCode.InternalServerError => $"Error occurred during product creation!",
        };

    private async Task<string?> GetIdentityUserIdFromCache(string token, CancellationToken cancellationToken = default) =>
        await tokenService.GetIdentityUserIdByToken(token, cancellationToken);
}