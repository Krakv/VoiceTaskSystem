using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Interfaces;

public interface IRuleRepository
{
    Task<List<RuleItem>> GetRules(Guid userId, RuleEvent evt, CancellationToken ct);
    Task SaveChanges(CancellationToken ct);
}
