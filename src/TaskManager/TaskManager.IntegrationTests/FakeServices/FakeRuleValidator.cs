using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.IntegrationTests.FakeServices;

public class FakeRuleValidator : IRuleValidator
{
    public void Validate(RuleEvent ruleEvent, ConditionGroup? condition, IEnumerable<RuleAction> actions)
    {
        
    }
}
