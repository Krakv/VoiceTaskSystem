using FluentValidation;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.CommandRequest.Validators;

public class UpdateVoiceTaskDtoValidator : AbstractValidator<UpdateVoiceTaskDto>
{
    public UpdateVoiceTaskDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("Некорректный TaskId");

        RuleFor(x => x.ParentTaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("Некорректный ParentTaskId");

        RuleFor(x => x.Status)
            .Must(SimpleValidators.BeValidEnumOrNull<TaskItemStatus>)
            .WithMessage("Некорректный Status");

        RuleFor(x => x.Priority)
            .Must(SimpleValidators.BeValidEnumOrNull<TaskItemPriority>)
            .WithMessage("Некорректный Priority");

        RuleFor(x => x.DueDate)
            .Must(SimpleValidators.BeValidDateTimeOffsetOrNull)
            .WithMessage("Некорректная дата");

        RuleFor(x => x.Title)
            .MaximumLength(100);

        RuleFor(x => x.ProjectName)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
