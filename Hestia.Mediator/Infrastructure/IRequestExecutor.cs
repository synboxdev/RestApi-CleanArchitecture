using Hestia.Mediator.Infrastructure.Messaging;

namespace Hestia.Mediator.Infrastructure;

public interface IRequestExecutor
{
    Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
}