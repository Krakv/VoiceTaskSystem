using FluentValidation;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Pipeline;

namespace TaskManager.ApiGateway.DTOs.Rule.Validators;

public class UpdateRuleDtoValidator : AbstractValidator<UpdateRuleDto>
{
    public UpdateRuleDtoValidator()
    {
        RuleFor(x => x.RuleEvent)
            .NotEmpty()
            .Must(SimpleValidators.BeValidEnum<RuleEvent>)
            .WithMessage("Некорректный RuleEvent");

        RuleFor(x => x.Actions)
            .NotNull()
            .Must(a => a.Any())
            .WithMessage("Actions не может быть пустым");

        RuleFor(x => x.Conditions)
            .Must(SimpleValidators.BeValidJsonOrNull)
            .WithMessage("Conditions некорректный JSON");

        RuleForEach(x => x.Actions)
            .Must(SimpleValidators.BeValidJson)
            .WithMessage("Некорректный Action JSON");
    }
}
