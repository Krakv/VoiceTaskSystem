using System.Globalization;
using System.Reflection;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleEvaluator() : IRuleEvaluator
{
    private static readonly Dictionary<string, ComparisonOperator[]> AllowedOperators =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Title"] = [ComparisonOperator.eq, ComparisonOperator.neq],
            ["Status"] = [ComparisonOperator.eq, ComparisonOperator.neq],
            ["ParentTaskId"] = [ComparisonOperator.eq, ComparisonOperator.neq],
            ["Priority"] = [ComparisonOperator.eq, ComparisonOperator.neq, ComparisonOperator.gt, ComparisonOperator.lt],
            ["DueDate"] = [ComparisonOperator.eq, ComparisonOperator.neq, ComparisonOperator.gt, ComparisonOperator.lt],
            ["CreatedAt"] = [ComparisonOperator.eq, ComparisonOperator.neq, ComparisonOperator.gt, ComparisonOperator.lt],
        };

    public bool Evaluate(TaskItem task, ConditionGroup? conditionGroup)
    {
        return EvaluateGroup(task, conditionGroup);
    }

    private static bool EvaluateGroup(TaskItem task, ConditionGroup? group)
    {
        if (group == null) return true;

        var results = group.Conditions.Select(c => EvaluateCondition(task, c)).ToList();

        return group.Operator switch
        {
            LogicalOperator.AND => results.All(x => x),
            LogicalOperator.OR => results.Any(x => x),
            _ => false
        };
    }

    private static bool EvaluateCondition(TaskItem task, Condition condition)
    {
        if (AllowedOperators.TryGetValue(condition.Field, out var allowed)
            && !allowed.Contains(condition.Operator))
        {
            return false;
        }

        var prop = task.GetType().GetProperty(condition.Field,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop == null) return false;

        var taskValue = prop.GetValue(task);

        if (taskValue == null) return false;

        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (!TryConvert(condition.Value, targetType, out var condValue))
            return false;

        return condition.Operator switch
        {
            ComparisonOperator.eq => Equals(taskValue, condValue),
            ComparisonOperator.neq => !Equals(taskValue, condValue),
            ComparisonOperator.gt => TryCompare(taskValue, condValue, out var r) && r > 0,
            ComparisonOperator.lt => TryCompare(taskValue, condValue, out var r) && r < 0,
            _ => false
        };
    }

    private static bool TryConvert(string raw, Type targetType, out object? result)
    {
        result = null;
        try
        {
            result = targetType switch
            {
                _ when targetType.IsEnum
                    => Enum.Parse(targetType, raw, ignoreCase: true),

                _ when targetType == typeof(DateTimeOffset)
                    => string.IsNullOrEmpty(raw) ? null : DateTimeOffset.Parse(raw, CultureInfo.InvariantCulture),

                _ when targetType == typeof(DateTime)
                    => string.IsNullOrEmpty(raw) ? null : DateTime.Parse(raw, CultureInfo.InvariantCulture),

                _ when targetType == typeof(Guid)
                    => string.IsNullOrEmpty(raw) ? null : Guid.Parse(raw),

                _ => Convert.ChangeType(raw, targetType)
            };
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryCompare(object a, object? b, out int result)
    {
        result = 0;
        if (a is IComparable comparable)
        {
            try
            {
                result = comparable.CompareTo(b);
                return true;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
}
