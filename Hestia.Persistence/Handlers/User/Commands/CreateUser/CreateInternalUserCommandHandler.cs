using Hestia.Access.Requests.User.Commands.CreateUser;
using Hestia.Persistence.Contexts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.User.Commands.CreateUser;

internal class CreateInternalUserCommandHandler(HestiaContext context, ILogger<CreateInternalUserCommandHandler> logger) : IRequestHandler<CreateInternalUserCommand, bool>
{
    public async Task<bool> Handle(CreateInternalUserCommand request, CancellationToken cancellationToken) =>
        await context.ExecuteInTransactionAsync(async context =>
        {
            await context.User.AddAsync(request.User, cancellationToken);
            return await context.SaveChangesAsync(cancellationToken);
        }, logger) != 0;
}