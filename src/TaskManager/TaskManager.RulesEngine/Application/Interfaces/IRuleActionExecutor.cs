using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleActionExecutor
{
    Task ExecuteAsync(TaskItem task, RuleAction action);
}
