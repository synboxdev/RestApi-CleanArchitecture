using MediatR;

namespace Hestia.Access.Requests.Product.Queries.GetExisting;

public sealed record GetExistingProductByUserQuery(string UserId, string ExternalId) : IRequest<Entities.Product.Product?>;