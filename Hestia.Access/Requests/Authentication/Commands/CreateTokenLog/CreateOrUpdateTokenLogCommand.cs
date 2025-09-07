using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Types;

namespace Hestia.Access.Requests.Authentication.Commands.CreateTokenLog;

public sealed record CreateOrUpdateTokenLogCommand(string Token, string IdentityUserId, DateTime TokenExpirationDate) : IRequest<Unit>;