using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRulePolicyValidator
{
    void Validate(RuleEvent ruleEvent, IEnumerable<RuleAction> actions);
}
