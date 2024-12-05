using Hestia.Domain.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hestia.Access.Requests.User.Commands.CreateUser;

public sealed record CreateApplicationUserCommand((ApplicationUser, string) User) : IRequest<IdentityResult>;