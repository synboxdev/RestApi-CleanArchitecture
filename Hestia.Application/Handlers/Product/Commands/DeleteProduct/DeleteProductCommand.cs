using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using MediatR;

namespace Hestia.Application.Handlers.Product.Commands.DeleteProduct;

public sealed record DeleteProductCommand(GetProductDto Product) : IRequest<ApiResponse<bool>>;