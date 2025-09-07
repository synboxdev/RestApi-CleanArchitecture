using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Application.Handlers.Product.Commands.CreateProduct;

public sealed record CreateProductCommand(CreateProductDto Product, string Token) : IRequest<ApiResponse<Guid?>>;