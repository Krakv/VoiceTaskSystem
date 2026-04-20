using FluentValidation;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.CalendarEvent.Validators;

public class CreateCalendarEventDtoValidator : AbstractValidator<CreateCalendarEventDto>
{
    public CreateCalendarEventDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название обязательно");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Дата начала обязательна")
            .Must(SimpleValidators.BeValidDateTimeOffset)
            .WithMessage("Некорректный формат даты начала");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Дата окончания обязательна")
            .Must(SimpleValidators.BeValidDateTimeOffset)
            .WithMessage("Некорректный формат даты окончания");

        RuleFor(x => x.TaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("TaskId должен быть корректным GUID");

        RuleFor(x => x.ExternalAccountId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("ExternalAccountId должен быть корректным GUID");
    }
}
