using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByExternalIdQueryHandler(RheaContext context, ILogger<GetExistingProductByExternalIdQueryHandler> logger) :
    IRequestHandler<GetExistingProductByExternalIdQuery, Access.Entities.Product.Product?>
{
    public async Task<Access.Entities.Product.Product?> Handle(GetExistingProductByExternalIdQuery request, CancellationToken cancellationToken)
    {
        var existingProduct = await context.ExecuteInTransactionAsync(async context =>
        {
            return await context.Product.FirstOrDefaultAsync(x => x.ExternalId == request.ExternalId, cancellationToken);
        }, logger);

        return existingProduct;
    }
}