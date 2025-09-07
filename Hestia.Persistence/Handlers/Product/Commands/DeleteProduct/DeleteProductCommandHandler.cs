using Hestia.Access.Requests.Product.Commands.DeleteProduct;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Product.Commands.DeleteProduct;

public class DeleteProductCommandHandler(RheaContext context, ILogger<DeleteProductCommandHandler> logger) :
    IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken) =>
        await context.ExecuteInTransactionAsync(async context =>
        {
            context.Product.Remove(request.Product);
            return await context.SaveChangesAsync(cancellationToken);
        }, logger) != 0;
}