using FluentValidation;

namespace TaskManager.Auth.Application.Features.Auth.UpdateUserProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name != null);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Некорректный email")
            .When(x => x.Email != null);
    }
}
