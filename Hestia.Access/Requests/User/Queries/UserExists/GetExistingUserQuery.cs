using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Access.Requests.User.Queries.UserExists;

public sealed record GetExistingUserQuery(string Username, string Email) : IRequest<ApplicationUser?>;