using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.CreateProduct;
using MediatR;

namespace Hestia.Application.Handlers.Product.Commands.CreateProduct;

public sealed record CreateProductCommand(CreateProductDto Product, string Token) : IRequest<ApiResponse<Guid?>>;