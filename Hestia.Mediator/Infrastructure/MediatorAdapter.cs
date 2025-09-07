using Hestia.Mediator.Infrastructure.Layers;
using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Mediator.Infrastructure;

public class MediatorAdapter(IMediator mediator) : IAccessLayer, ICoreLayer
{
    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
        mediator.Send(request, cancellationToken);

    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest =>
        mediator.Send(request, cancellationToken);
}