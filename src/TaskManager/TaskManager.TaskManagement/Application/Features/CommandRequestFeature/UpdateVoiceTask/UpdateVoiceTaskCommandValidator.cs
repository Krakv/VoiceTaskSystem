using FluentValidation;
using TaskManager.Shared.Pipeline;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

public class UpdateVoiceTaskCommandValidator : AbstractValidator<UpdateVoiceTaskCommand>
{
    public UpdateVoiceTaskCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.CommandRequestId)
            .NotEmpty();

        RuleFor(x => x.TaskId)
            .NotEmpty()
            .When(x => x.TaskId.HasValue);

        RuleFor(x => x.DueDate)
            .Must(SimpleValidators.BeValidDateTimeOffsetOrNull)
            .WithMessage("Некорректная дата");
    }
}
