using FluentValidation;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Rule.Validators;

public class CreateRuleDtoValidator : AbstractValidator<CreateRuleDto>
{
    public CreateRuleDtoValidator()
    {
        RuleFor(x => x.RuleEvent)
            .NotEmpty().WithMessage("RuleEvent обязателен")
            .Must(SimpleValidators.BeValidEnum<RuleEvent>)
            .WithMessage("Некорректный RuleEvent");

        RuleFor(x => x.Conditions)
            .Must(SimpleValidators.BeValidJsonOrNull)
            .WithMessage("Conditions некорректный JSON");

        RuleForEach(x => x.Actions)
            .Must(SimpleValidators.BeValidJson)
            .WithMessage("Action имеет некорректный JSON");
    }
}
