using MediatR;

namespace Hestia.Access.Requests.Authentication.Commands.CreateTokenLog;

public sealed record CreateOrUpdateTokenLogCommand(string Token, string IdentityUserId, DateTime TokenExpirationDate) : IRequest<Unit>;