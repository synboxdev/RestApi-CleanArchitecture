using Hestia.Access.Entities.User;
using Hestia.Access.Requests.User.Commands.CreateUser;
using Hestia.Application.Interfaces.Authentication;
using Hestia.Application.Models.Authentication.Inbound;
using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Layers;
using Microsoft.AspNetCore.Identity;

namespace Hestia.Application.Services.Authentication;

public class UserService(IAccessLayer accessLayer) : IUserService
{
    public async Task<ApplicationUser?> CreateUserAsync(ApplicationUserRegisterDto model, CancellationToken cancellationToken = default)
    {
        var applicationUser = CreateApplicationUser(model);
        var applicationUserCreateResult = await CreateApplicationUserAsync(applicationUser, model.Password, cancellationToken);

        if (applicationUserCreateResult.Succeeded)
        {
            var internalUser = CreateInternalUser(applicationUser.Id, model);
            bool internalUserCreateResult = await CreateInternalUserAsync(internalUser, cancellationToken);

            if (internalUserCreateResult)
                return applicationUser;
        }

        return null;
    }

    private static ApplicationUser CreateApplicationUser(ApplicationUserRegisterDto model) =>
        new()
        {
            UserName = model.Username,
            Email = model.Email,
            Role = model.Role
        };

    private static User CreateInternalUser(string identityUserId, ApplicationUserRegisterDto model) =>
        new(Guid.NewGuid())
        {
            IdentityUserId = identityUserId,
            DateCreated = DateTime.UtcNow,
            Name = model.Name
        };

    private async Task<IdentityResult> CreateApplicationUserAsync(ApplicationUser User, string Password, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new CreateApplicationUserCommand((User, Password)), cancellationToken);

    private async Task<bool> CreateInternalUserAsync(User User, CancellationToken cancellationToken = default) =>
        await accessLayer.ExecuteAsync(new CreateInternalUserCommand(User), cancellationToken);
}