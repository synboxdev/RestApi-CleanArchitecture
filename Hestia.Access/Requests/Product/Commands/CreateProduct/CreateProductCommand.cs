using MediatR;

namespace Hestia.Access.Requests.Product.Commands.CreateProduct;

public sealed record CreateProductCommand(Entities.Product.Product Product) : IRequest<bool>;