using FluentValidation;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Task.Validators;

public class GetTasksDtoValidator : AbstractValidator<GetTasksDto>
{
    public GetTasksDtoValidator()
    {
        RuleFor(x => x.Status)
            .Must(SimpleValidators.BeValidEnumOrNull<TaskItemStatus>)
            .WithMessage("Status некорректен");

        RuleFor(x => x.Priority)
            .Must(SimpleValidators.BeValidEnumOrNull<TaskItemPriority>)
            .WithMessage("Priority некорректен");

        RuleFor(x => x.Limit)
            .Must(SimpleValidators.BeValidIntPositive)
            .WithMessage("Limit должен быть числом > 0");

        RuleFor(x => x.Page)
            .Must(SimpleValidators.BeValidIntNonNegative)
            .WithMessage("Page должен быть числом >= 0");
    }
}
