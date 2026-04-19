using FluentValidation;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Notification.Validators;

public sealed class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
{
    public CreateNotificationDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .Must(SimpleValidators.BeValidGuidOrNull)
            .WithMessage("TaskId должен быть корректным GUID");

        RuleFor(x => x.ScheduledAt)
            .NotEmpty().WithMessage("Дата обязательна")
            .Must(SimpleValidators.BeValidDateTimeOffset)
            .WithMessage("Некорректный формат даты");

        RuleFor(x => x.ServiceId)
            .IsInEnum().WithMessage("Некорректный тип сервиса");
    }
}
