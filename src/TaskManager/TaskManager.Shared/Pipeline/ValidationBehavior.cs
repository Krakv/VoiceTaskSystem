using FluentValidation;
using MediatR;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Shared.Pipeline;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures
            .SelectMany(f => f.Errors)
            .Where(f => f != null)
            .ToList();

        if (errors.Count != 0)
        {
            var errorsDictionary = errors
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).First());

            throw new ValidationAppException("INVALID_PARAMS", errorsDictionary);
        }

        return await next(cancellationToken);
    }
}
