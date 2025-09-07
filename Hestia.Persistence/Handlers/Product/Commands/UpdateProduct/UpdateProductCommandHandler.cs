using Hestia.Access.Requests.Product.Commands.UpdateProduct;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler(RheaContext context, ILogger<UpdateProductCommandHandler> logger) :
    IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken) =>
        await context.ExecuteInTransactionAsync(async context =>
        {
            context.Product.Update(request.Product);
            return await context.SaveChangesAsync(cancellationToken);
        }, logger) != 0;
}