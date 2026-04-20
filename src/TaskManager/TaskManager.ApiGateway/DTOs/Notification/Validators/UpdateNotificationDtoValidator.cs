using FluentValidation;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Notification.Validators;

public class UpdateNotificationDtoValidator : AbstractValidator<UpdateNotificationDto>
{
    public UpdateNotificationDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.ScheduledAt)
            .Must(SimpleValidators.BeValidDateTimeOffset);
    }
}
