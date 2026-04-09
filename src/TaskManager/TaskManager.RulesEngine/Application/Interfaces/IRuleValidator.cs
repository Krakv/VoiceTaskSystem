using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleValidator
{
    void Validate(
        RuleEvent ruleEvent,
        ConditionGroup condition,
        IEnumerable<RuleAction> actions);
}
