using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Access.Requests.Product.Commands.UpdateProduct;

public sealed record UpdateProductCommand(Entities.Product.Product Product) : IRequest<bool>;