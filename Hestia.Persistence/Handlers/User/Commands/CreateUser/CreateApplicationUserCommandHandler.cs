using Hestia.Access.Requests.User.Commands.CreateUser;
using Hestia.Domain.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hestia.Persistence.Handlers.User.Commands.CreateUser;

internal class CreateApplicationUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<CreateApplicationUserCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(CreateApplicationUserCommand request, CancellationToken cancellationToken) =>
        await userManager.CreateAsync(request.User.Item1, request.User.Item2);
}