using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleDomainValidator
{
    void Validate(ConditionGroup? condition, IEnumerable<RuleAction> actions);
}
