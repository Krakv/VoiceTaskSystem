using FluentValidation;

namespace TaskManager.RulesEngine.Application.Features.RuleFeature.CreateRule;

public sealed class CreateRuleCommandValidator : AbstractValidator<CreateRuleCommand>
{
    public CreateRuleCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId обязателен");

        RuleFor(x => x.RuleEvent)
            .IsInEnum().WithMessage("Некорректный тип события");

        RuleFor(x => x.Actions)
            .NotNull().WithMessage("Actions не может быть null")
            .Must(a => a.Any()).WithMessage("Actions не может быть пустым");
    }
}
