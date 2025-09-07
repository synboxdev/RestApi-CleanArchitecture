using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Messaging;
using Microsoft.AspNetCore.Identity;

namespace Hestia.Access.Requests.User.Commands.CreateUser;

public sealed record CreateApplicationUserCommand((ApplicationUser, string) User) : IRequest<IdentityResult>;