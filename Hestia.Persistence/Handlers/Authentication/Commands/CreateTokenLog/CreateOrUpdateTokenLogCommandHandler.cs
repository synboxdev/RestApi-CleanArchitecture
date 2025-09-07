using Hestia.Access.Entities.Authentication;
using Hestia.Access.Requests.Authentication.Commands.CreateTokenLog;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Types;
using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.Authentication.Commands.CreateTokenLog;

public class CreateOrUpdateTokenLogCommandHandler(HestiaContext context, ILogger<CreateOrUpdateTokenLogCommandHandler> logger) : IRequestHandler<CreateOrUpdateTokenLogCommand, Unit>
{
    public async Task<Unit> Handle(CreateOrUpdateTokenLogCommand request, CancellationToken cancellationToken)
    {
        await context.ExecuteInTransactionAsync(async async =>
        {
            var existingTokenLog = await context.TokenLog.FirstOrDefaultAsync(t => t.IdentityUserId == request.IdentityUserId, cancellationToken);

            if (existingTokenLog != null)
            {
                existingTokenLog.Token = request.Token;
                existingTokenLog.TokenExpirationDate = request.TokenExpirationDate;
                existingTokenLog.DateCreated = DateTime.UtcNow;

                context.TokenLog.Update(existingTokenLog);
            }
            else
            {
                var newTokenLog = new TokenLog(Guid.NewGuid())
                {
                    IdentityUserId = request.IdentityUserId,
                    Token = request.Token,
                    DateCreated = DateTime.UtcNow,
                    TokenExpirationDate = request.TokenExpirationDate
                };

                await context.TokenLog.AddAsync(newTokenLog, cancellationToken);
            }

            return await context.SaveChangesAsync(cancellationToken);
        }, logger);

        return Unit.Value;
    }
}