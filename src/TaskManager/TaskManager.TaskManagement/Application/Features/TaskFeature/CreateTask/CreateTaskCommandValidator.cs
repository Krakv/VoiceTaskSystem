using FluentValidation;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название задачи обязательно")
            .MaximumLength(100).WithMessage("Название задачи не должно превышать 100 символов");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description != null)
            .WithMessage("Описание не должно превышать 1000 символов");

        RuleFor(x => x.ProjectName)
            .MaximumLength(100)
            .When(x => x.ProjectName != null)
            .WithMessage("Название проекта слишком длинное");

        RuleFor(x => x.DueDate)
            .Must(date => date == null || date > DateTimeOffset.UtcNow)
            .WithMessage("Срок выполнения должен быть в будущем");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Некорректный статус");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Некорректный приоритет");
    }
}
