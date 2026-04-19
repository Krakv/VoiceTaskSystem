using FluentValidation;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Task.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи обязательно");

        RuleFor(x => x.DueDate)
            .Must(SimpleValidators.BeValidDateTimeOffsetOrNull)
            .WithMessage("Некорректный формат даты");

        RuleFor(x => x.ParentTaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("ParentTaskId должен быть корректным GUID");
    }
}
