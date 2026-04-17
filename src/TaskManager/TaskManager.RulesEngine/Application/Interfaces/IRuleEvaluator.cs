using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleEvaluator
{
    bool Evaluate(TaskItem task, ConditionGroup? conditionGroup);
}
