using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.GetProduct;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Application.Handlers.Product.Commands.DeleteProduct;

public sealed record DeleteProductCommand(GetProductDto Product) : IRequest<ApiResponse<bool>>;