using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Access.Requests.Product.Queries.GetExisting;

public sealed record GetExistingProductByCompositeIdQuery(Guid Id, string ExternalId) : IRequest<Entities.Product.Product?>;