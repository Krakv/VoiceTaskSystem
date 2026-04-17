using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleRepository(AppDbContext db) : IRuleRepository
{
    public Task<List<RuleItem>> GetRules(Guid userId, RuleEvent evt, CancellationToken ct)
        => db.RuleItem
            .Where(r => r.OwnerId == userId && r.Event == evt && r.IsActive)
            .ToListAsync(ct);

    public Task SaveChanges(CancellationToken ct)
        => db.SaveChangesAsync(ct);
}
