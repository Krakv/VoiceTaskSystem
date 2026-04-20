using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Shared.Pipeline;

public class AutoValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(arg);
            var result = await validator.ValidateAsync(validationContext);

            var errors = result.Errors.Where(e => e != null).ToList();
            if (errors.Count == 0) continue;

            var dict = errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).First());

            throw new ValidationAppException("INVALID_PARAMS", dict);
        }

        await next();
    }
}
