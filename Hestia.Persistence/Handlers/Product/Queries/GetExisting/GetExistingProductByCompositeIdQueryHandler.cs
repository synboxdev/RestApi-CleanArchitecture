using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByCompositeIdQueryHandler(RheaContext context, ILogger<GetExistingProductByCompositeIdQueryHandler> logger) :
    IRequestHandler<GetExistingProductByCompositeIdQuery, Access.Entities.Product.Product?>
{
    public async Task<Access.Entities.Product.Product?> Handle(GetExistingProductByCompositeIdQuery request, CancellationToken cancellationToken)
    {
        var existingProduct = await context.ExecuteInTransactionAsync(async context =>
        {
            return await context.Product.FirstOrDefaultAsync(x => x.Id == request.Id && x.ExternalId == request.ExternalId, cancellationToken);
        }, logger);

        return existingProduct;
    }
}