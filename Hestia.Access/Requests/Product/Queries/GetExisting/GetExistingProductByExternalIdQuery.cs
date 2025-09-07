using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Access.Requests.Product.Queries.GetExisting;

public sealed record GetExistingProductByExternalIdQuery(string ExternalId) : IRequest<Entities.Product.Product?>;