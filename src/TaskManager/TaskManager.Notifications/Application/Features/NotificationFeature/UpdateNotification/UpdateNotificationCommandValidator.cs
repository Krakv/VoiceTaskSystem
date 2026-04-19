using FluentValidation;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;

public class UpdateNotificationCommandValidator : AbstractValidator<UpdateNotificationCommand>
{
    public UpdateNotificationCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание обязательно")
            .MaximumLength(255).WithMessage("Описание не должно превышать 255 символов");

        RuleFor(x => x.ScheduledAt)
            .Must(date => date > DateTimeOffset.UtcNow)
            .WithMessage("Нельзя установить дату в прошлом");
    }
}
