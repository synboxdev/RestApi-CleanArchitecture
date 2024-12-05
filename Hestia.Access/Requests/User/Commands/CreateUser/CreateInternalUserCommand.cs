using MediatR;

namespace Hestia.Access.Requests.User.Commands.CreateUser;

public sealed record CreateInternalUserCommand(Entities.User.User User) : IRequest<bool>;