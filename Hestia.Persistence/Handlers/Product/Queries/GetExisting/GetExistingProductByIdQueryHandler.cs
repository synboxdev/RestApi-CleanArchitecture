using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByIdQueryHandler(RheaContext context, ILogger<GetExistingProductByIdQueryHandler> logger) :
    IRequestHandler<GetExistingProductByIdQuery, Access.Entities.Product.Product?>
{
    public async Task<Access.Entities.Product.Product?> Handle(GetExistingProductByIdQuery request, CancellationToken cancellationToken)
    {
        var existingProduct = await context.ExecuteInTransactionAsync(async context =>
        {
            return await context.Product.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        }, logger);

        return existingProduct;
    }
}