using Hestia.Access.Requests.User.Queries.ValidateUserLogin;
using Hestia.Domain.Models.Authentication;
using Hestia.Persistence.Contexts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Handlers.User.Queries.ValidateUserLogin;

internal class ValidateUserLoginQueryHandler(HestiaContext context, UserManager<ApplicationUser> userManager, ILogger<ValidateUserLoginQuery> logger) : IRequestHandler<ValidateUserLoginQuery, bool>
{
    public async Task<bool> Handle(ValidateUserLoginQuery request, CancellationToken cancellationToken) =>
        await userManager.CheckPasswordAsync(request.User, request.LoginPassword!);
}