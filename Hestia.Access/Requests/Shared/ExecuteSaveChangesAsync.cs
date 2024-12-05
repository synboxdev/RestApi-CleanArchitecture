using MediatR;

namespace Hestia.Access.Requests.Shared;

public sealed record ExecuteSaveChangesAsync : IRequest<Unit>;