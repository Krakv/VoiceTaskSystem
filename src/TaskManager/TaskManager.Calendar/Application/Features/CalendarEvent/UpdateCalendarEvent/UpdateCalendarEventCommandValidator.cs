using FluentValidation;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;

public class UpdateCalendarEventCommandValidator : AbstractValidator<UpdateCalendarEventCommand>
{
    public UpdateCalendarEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название обязательно")
            .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

        RuleFor(x => x.StartTime)
            .Must(x => x > DateTimeOffset.UtcNow)
            .WithMessage("Начало события должно быть в будущем");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("Дата окончания должна быть позже начала");

        RuleFor(x => x.Location)
            .MaximumLength(300)
            .When(x => x.Location != null)
            .WithMessage("Локация слишком длинная");
    }
}
