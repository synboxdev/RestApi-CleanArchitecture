using Hestia.Mediator.Infrastructure;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Types;
using System.Reflection;

namespace Hestia.Mediator.Services;

public class MediatorService(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

        object? handler = serviceProvider.GetService(handlerType) ??
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name} -> {typeof(TResponse).Name}");

        var method = handlerType.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public) ??
            throw new InvalidOperationException($"Handler {handlerType.Name} does not have a public Handle method");

        try
        {
            object? result = method.Invoke(handler, [request, cancellationToken]);
            return await (Task<TResponse>)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(Unit));

        object? handler = serviceProvider.GetService(handlerType) ??
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name} -> {nameof(Unit)}");

        var method = handlerType.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public) ??
            throw new InvalidOperationException($"Handler {handlerType.Name} does not have a public Handle method");

        try
        {
            object? result = method.Invoke(handler, [request, cancellationToken]);
            await (Task)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }
}
