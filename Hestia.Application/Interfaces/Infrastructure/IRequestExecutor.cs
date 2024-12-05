using MediatR;

namespace Hestia.Application.Interfaces.Infrastructure;

public interface IRequestExecutor
{
    Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
}