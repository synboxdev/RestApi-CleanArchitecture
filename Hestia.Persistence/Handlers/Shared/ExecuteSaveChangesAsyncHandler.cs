using Hestia.Access.Requests.Shared;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Types;
using Hestia.Persistence.Contexts;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Shared;

internal class ExecuteSaveChangesAsyncHandler(HestiaContext context, ILogger<ExecuteSaveChangesAsyncHandler> logger) :
    IRequestHandler<ExecuteSaveChangesAsync, Unit>
{
    public async Task<Unit> Handle(ExecuteSaveChangesAsync request, CancellationToken cancellationToken)
    {
        await context.ExecuteInTransactionAsync(async context =>
        {
            return await context.SaveChangesAsync();
        }, logger);

        return Unit.Value;
    }
}