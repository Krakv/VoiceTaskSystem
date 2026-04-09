using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleApplier
{
    Task ApplyRulesAsync(TaskItem task, RuleEvent ruleEvent, Guid userId, CancellationToken ct = default);
}
