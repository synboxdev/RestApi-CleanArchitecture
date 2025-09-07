using Hestia.Access.Requests.User.Queries.UserExists;
using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.User.Queries.UserExists;

internal class GetExistingUserQueryHandler(HestiaContext context, ILogger<GetExistingUserQueryHandler> logger, UserManager<ApplicationUser> userManager) :
    IRequestHandler<GetExistingUserQuery, ApplicationUser?>
{
    public async Task<ApplicationUser?> Handle(GetExistingUserQuery request, CancellationToken cancellationToken)
    {
        var existingUser = await context.ExecuteInTransactionAsync(async context =>
        {
            var userByEmail = await userManager.FindByEmailAsync(request.Email!);
            var userByName = await userManager.FindByNameAsync(request.Username!);
            return userByEmail is not null && userByName is not null && userByEmail.Equals(userByName) ? userByEmail : null;
        }, logger);

        return existingUser;
    }
}