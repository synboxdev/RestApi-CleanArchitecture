using Hestia.Access.Requests.Product.Queries.GetExisting;
using Hestia.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Queries.GetExisting;

public class GetExistingProductByUserQueryHandler(RheaContext context, ILogger<GetExistingProductByUserQueryHandler> logger) :
    IRequestHandler<GetExistingProductByUserQuery, Access.Entities.Product.Product?>
{
    public async Task<Access.Entities.Product.Product?> Handle(GetExistingProductByUserQuery request, CancellationToken cancellationToken)
    {
        var existingProduct = await context.ExecuteInTransactionAsync(async context =>
        {
            return await context.Product.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ExternalId == request.ExternalId, cancellationToken);
        }, logger);

        return existingProduct;
    }
}