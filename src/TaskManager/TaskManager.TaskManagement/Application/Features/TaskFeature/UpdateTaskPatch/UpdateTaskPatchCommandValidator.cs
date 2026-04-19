using FluentValidation;
using TaskManager.Shared.Pipeline;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

public sealed class UpdateTaskPatchCommandValidator : AbstractValidator<UpdateTaskPatchCommand>
{
    public UpdateTaskPatchCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .MaximumLength(100)
            .When(x => x.ProjectName != null);

        RuleFor(x => x.Title)
            .MaximumLength(100)
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description != null);

        RuleFor(x => x.DueDate)
            .Must(SimpleValidators.BeValidDateTimeOffsetOrNull)
            .WithMessage("Некорректная дата");

        RuleFor(x => x.ParentTaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("Некорректный GUID");
    }
}
