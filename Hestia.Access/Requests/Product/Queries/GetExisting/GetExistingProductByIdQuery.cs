using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Access.Requests.Product.Queries.GetExisting;

public sealed record GetExistingProductByIdQuery(Guid? Id) : IRequest<Entities.Product.Product?>;