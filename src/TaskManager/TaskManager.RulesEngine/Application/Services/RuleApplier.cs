using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using TaskManager.Repository.Context;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Utils;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleApplier(AppDbContext dbContext, ILogger<RuleApplier> logger) : IRuleApplier
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<RuleApplier> _logger = logger;

    public async Task ApplyRulesAsync(TaskItem task, RuleEvent ruleEvent, Guid userId, CancellationToken ct = default)
    {
        var rules = await _dbContext.RuleItem
            .Where(r => r.OwnerId == userId && r.Event == ruleEvent && r.IsActive)
            .ToListAsync(ct);

        _logger.LogInformation("Found {Count} active rules for event {Event}", rules.Count, ruleEvent);

        foreach (var rule in rules)
        {
            _logger.LogDebug("Evaluating rule: {RuleId}", rule.RuleId);

            ConditionGroup? conditionGroup = null;
            RuleAction[]? actions = null;

            try
            {
                conditionGroup = JsonSerializer.Deserialize<ConditionGroup>(rule.Condition, JsonHelper.Default);
                actions = JsonSerializer.Deserialize<RuleAction[]>(rule.Action, JsonHelper.Default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize rule {RuleId}", rule.RuleId);
                continue;
            }

            if (conditionGroup == null || actions == null)
            {
                _logger.LogWarning("Rule {RuleId} has invalid condition or actions", rule.RuleId);
                continue;
            }

            // Проверяем условие
            if (!EvaluateCondition(task, conditionGroup))
            {
                _logger.LogDebug("Rule {RuleId} condition not met", rule.RuleId);
                continue;
            }

            _logger.LogInformation("Rule {RuleId} condition met, applying actions", rule.RuleId);

            // Применяем действия
            foreach (var action in actions)
            {
                try
                {
                    await ExecuteActionAsync(task, action);
                    _logger.LogDebug("Applied action {ActionType} on TaskId: {TaskId}", action.Type, task.TaskId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to apply action {ActionType} for rule {RuleId}", action.Type, rule.RuleId);
                }
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Finished applying rules for TaskId: {TaskId}", task.TaskId);
    }

    private bool EvaluateCondition(TaskItem task, ConditionGroup group)
    {
        var results = group.Conditions.Select(c =>
        {
            if (c is Condition cond)
            {
                var value = task.GetType().GetProperty(cond.Field,BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.GetValue(task)?.ToString()?.ToLower();
                _logger.LogDebug("Evaluating condition: {Field} {Operator} {Value} (Task value: {TaskValue})", cond.Field, cond.Operator, cond.Value, value);
                return cond.Operator.ToString() switch
                {
                    "eq" => value == cond.Value,
                    "neq" => value != cond.Value,
                    "gt" => string.Compare(value, cond.Value) > 0,
                    "lt" => string.Compare(value, cond.Value) < 0,
                    _ => false
                };
            }

            return false;
        });

        return group.Operator.ToString().ToUpper() switch
        {
            "AND" => results.All(r => r),
            "OR" => results.Any(r => r),
            _ => false
        };
    }

    private async Task ExecuteActionAsync(TaskItem task, RuleAction action)
    {
        switch (action)
        {
            case SetFieldAction set:
                var prop = task.GetType().GetProperty(set.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop == null || !prop.CanWrite) break;
                _logger.LogDebug("Setting field {Field} to value {Value} on TaskId: {TaskId}", set.Field, set.Value, task.TaskId);

                Type targetType = prop.PropertyType;

                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                object? value;

                if (underlyingType == typeof(Guid))
                {
                    value = Guid.Parse(set.Value);
                }
                else if (underlyingType == typeof(DateTimeOffset))
                {
                    value = DateTimeOffset.Parse(set.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }
                else if (underlyingType.IsEnum)
                {
                    value = Enum.Parse(underlyingType, set.Value, true);
                }
                else
                {
                    value = Convert.ChangeType(set.Value, underlyingType);
                }

                prop.SetValue(task, value);
                break;

            case CreateNotificationAction notif:
                // TODO
                break;

            case CreateCalendarEventAction calendar:
                // TODO
                break;
        }

        await Task.CompletedTask;
    }
}
