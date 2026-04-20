using FluentValidation;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Task.Validators;

public class UpdateTaskPatchDtoValidator : AbstractValidator<UpdateTaskPatchDto>
{
    public UpdateTaskPatchDtoValidator()
    {
        RuleFor(x => x.Status)
            .Must(SimpleValidators.BeValidEnumOrNull<TaskItemStatus>)
            .WithMessage("Status некорректен");

        RuleFor(x => x.Priority)
            .Must(SimpleValidators.BeValidEnumOrNull<TaskItemPriority>)
            .WithMessage("Priority некорректен");

        RuleFor(x => x.DueDate)
            .Must(SimpleValidators.BeValidDateTimeOffsetOrNull)
            .WithMessage("DueDate должен быть корректной датой");

        RuleFor(x => x.ParentTaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("ParentTaskId должен быть корректным GUID");
    }
}
