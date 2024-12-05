using Hestia.Application.Models.Shared;
using Hestia.Domain.Models.Product.Inbound.UpdateProduct;
using MediatR;

namespace Hestia.Application.Handlers.Product.Commands.UpdateProduct;

public sealed record UpdateProductCommand(UpdateProductDto Product) : IRequest<ApiResponse<bool>>;