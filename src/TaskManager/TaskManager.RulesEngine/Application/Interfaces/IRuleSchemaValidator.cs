using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleSchemaValidator
{
    void ValidateStructure(ConditionGroup? condition, IEnumerable<RuleAction> actions);
}
