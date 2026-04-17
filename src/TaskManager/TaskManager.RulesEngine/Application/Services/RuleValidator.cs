using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleValidator(IRuleSchemaValidator ruleSchemaValidator, IRulePolicyValidator rulePolicyValidator, IRuleDomainValidator ruleDomainValidator) : IRuleValidator
{
    public void Validate(
        RuleEvent ruleEvent,
        ConditionGroup? condition,
        IEnumerable<RuleAction> actions)
    {
        ruleSchemaValidator.ValidateStructure(condition, actions);
        ruleDomainValidator.Validate(condition, actions);
        rulePolicyValidator.Validate(ruleEvent, actions);
    }
}
