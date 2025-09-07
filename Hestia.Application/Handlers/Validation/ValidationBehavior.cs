using FluentValidation;
using Hestia.Mediator.Infrastructure.Messaging;
using Hestia.Mediator.Infrastructure.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Hestia.Application.Handlers.Validation;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    IServiceProvider serviceProvider,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger) :
    IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        // Log the validators being used
        foreach (var validator in validators)
        {
            logger.LogInformation($"Using validator: {validator.GetType().Name}");
        }

        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        // Validate properties of TRequest
        var propertyFailures = new List<FluentValidation.Results.ValidationFailure>();
        var properties = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            object? propertyValue = property.GetValue(request);
            if (propertyValue != null)
            {
                var propertyValidatorType = typeof(IValidator<>).MakeGenericType(property.PropertyType);
                var propertyValidators = serviceProvider.GetServices(propertyValidatorType).Cast<IValidator>().ToList();

                // Log the property validators being used
                foreach (var propertyValidator in propertyValidators)
                {
                    logger.LogInformation($"Using property validator: {propertyValidator.GetType().Name} for property: {property.Name}");
                }

                foreach (var propertyValidator in propertyValidators)
                {
                    var validationResults = propertyValidator.Validate(new ValidationContext<object>(propertyValue));
                    propertyFailures.AddRange(validationResults.Errors.Where(f => f != null));
                }
            }
        }

        failures.AddRange(propertyFailures);

        return failures.Count != 0 ? throw new ValidationException(failures) : await next();
    }
}