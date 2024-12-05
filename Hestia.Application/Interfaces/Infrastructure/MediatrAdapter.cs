using MediatR;

namespace Hestia.Application.Interfaces.Infrastructure;

public class MediatrAdapter(IMediator mediator) : IAccessLayer, ICoreLayer
{
    public Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
        mediator.Send(request, cancellationToken);

    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest =>
        mediator.Send(request, cancellationToken);

    public Task PublishAsync<TNotification>(TNotification notification) where TNotification : INotification =>
        mediator.Publish(notification);
}