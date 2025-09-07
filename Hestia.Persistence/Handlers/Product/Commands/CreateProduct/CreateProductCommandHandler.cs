using Hestia.Access.Requests.Product.Commands.CreateProduct;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Hestia.Persistence.Handlers.Product.Queries.GetExisting;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Commands.CreateProduct;

public class CreateProductCommandHandler(RheaContext context, ILogger<GetExistingProductByUserQueryHandler> logger) :
    IRequestHandler<CreateProductCommand, bool>
{
    public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken) =>
        await context.ExecuteInTransactionAsync(async context =>
        {
            await context.Product.AddAsync(request.Product, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }, logger) != 0;
}