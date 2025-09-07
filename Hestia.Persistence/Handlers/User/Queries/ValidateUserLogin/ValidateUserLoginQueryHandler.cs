using Hestia.Access.Requests.User.Queries.ValidateUserLogin;
using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Hestia.Persistence.Handlers.User.Queries.ValidateUserLogin;

internal class ValidateUserLoginQueryHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<ValidateUserLoginQuery, bool>
{
    public async Task<bool> Handle(ValidateUserLoginQuery request, CancellationToken cancellationToken) =>
        await userManager.CheckPasswordAsync(request.User, request.LoginPassword!);
}