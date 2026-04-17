using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Utils;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleApplier(
    IRuleRepository repo,
    IRuleEvaluator evaluator,
    IRuleActionExecutor executor,
    ILogger<RuleApplier> logger) : IRuleApplier
{

    public async Task ApplyRulesAsync(TaskItem task, RuleEvent ruleEvent, Guid userId, CancellationToken ct = default)
    {
        var rules = await repo.GetRules(userId, ruleEvent, ct);

        logger.LogInformation("Found {Count} active rules for event {Event}", rules.Count, ruleEvent);

        foreach (var rule in rules)
        {
            logger.LogDebug("Evaluating rule: {RuleId}", rule.RuleId);

            ConditionGroup? conditionGroup = null;
            RuleAction[]? actions = null;

            try
            {
                conditionGroup = rule.Condition == null ? null : JsonSerializer.Deserialize<ConditionGroup>(rule.Condition, JsonHelper.Default);
                actions = JsonSerializer.Deserialize<RuleAction[]>(rule.Action, JsonHelper.Default);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to deserialize rule {RuleId}", rule.RuleId);
                continue;
            }

            if (actions == null)
            {
                logger.LogWarning("Rule {RuleId} has invalid actions", rule.RuleId);
                continue;
            }

            // Проверяем условие
            if (!evaluator.Evaluate(task, conditionGroup))
            {
                logger.LogDebug("Rule {RuleId} condition not met", rule.RuleId);
                continue;
            }

            logger.LogInformation("Rule {RuleId} condition met, applying actions", rule.RuleId);

            // Применяем действия
            foreach (var action in actions)
            {
                try
                {
                    await executor.ExecuteAsync(task, action);
                    logger.LogDebug("Applied action {ActionType} on TaskId: {TaskId}", action.Type, task.TaskId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to apply action {ActionType} for rule {RuleId}", action.Type, rule.RuleId);
                }
            }
        }

        await repo.SaveChanges(ct);
        logger.LogInformation("Finished applying rules for TaskId: {TaskId}", task.TaskId);
    }
}
