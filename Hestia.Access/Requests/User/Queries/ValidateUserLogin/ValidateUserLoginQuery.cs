using Hestia.Domain.Models.Authentication;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Access.Requests.User.Queries.ValidateUserLogin;

public sealed record ValidateUserLoginQuery(ApplicationUser User, string LoginPassword) : IRequest<bool>;