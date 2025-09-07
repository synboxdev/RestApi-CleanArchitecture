using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Types;

namespace Hestia.Access.Requests.Shared;

public sealed record ExecuteSaveChangesAsync : IRequest<Unit>;