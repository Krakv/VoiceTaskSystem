using System.Reflection;
using System.Globalization;
using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleDomainValidator : IRuleDomainValidator
{
    private const string InvalidFieldExceptionCode = "INVALID_FIELD";
    private const string InvalidValueExceptionCode = "INVALID_VALUE";
    private const string InvalidOperatorExceptionCode = "INVALID_OPERATOR";

    public void Validate(ConditionGroup? condition, IEnumerable<RuleAction> actions)
    {
        ValidateConditions(condition);
        ValidateActions(actions);
    }

    private static void ValidateConditions(ConditionGroup? condition)
    {
        if (condition?.Conditions == null)
            return;

        foreach (var c in condition.Conditions)
        {
            ValidateFieldExists(c.Field);
            ValidateValueType(c.Field, c.Value);
            ValidateOperator(c.Operator);
        }
    }

    private static void ValidateActions(IEnumerable<RuleAction> actions)
    {
        foreach (var action in actions)
        {
            switch (action)
            {
                case SetFieldAction set:
                    ValidateSetField(set);
                    break;

                case CreateNotificationAction notif:
                    ValidateNotification(notif);
                    break;

                case CreateCalendarEventAction calendar:
                    ValidateCalendar(calendar);
                    break;
            }
        }
    }

    private static void ValidateFieldExists(string field)
    {
        if (!TaskFieldMap.Allowed.Contains(field))
        {
            throw new ValidationAppException(InvalidFieldExceptionCode, $"Поле не существует: {field}");
        }
    }

    private static void ValidateValueType(string field, string value)
    {
        var prop = typeof(TaskItem).GetProperty(
            field,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new ValidationAppException(InvalidFieldExceptionCode, $"Поле не существует: {field}");
        
        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        try
        {
            if (targetType == typeof(Guid))
            {
                Guid.Parse(value);
            }
            else if (targetType == typeof(DateTimeOffset))
            {
                DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
            }
            else if (targetType.IsEnum)
            {
                Enum.Parse(targetType, value, true);
            }
            else
            {
                Convert.ChangeType(value, targetType);
            }
        }
        catch
        {
            throw new ValidationAppException(InvalidValueExceptionCode, $"Неверное значение '{value}' для поля '{field}'");
        }
    }

    private static void ValidateOperator(ComparisonOperator op)
    {
        var valid = op switch
        {
            ComparisonOperator.eq => true,
            ComparisonOperator.neq => true,
            ComparisonOperator.gt => true,
            ComparisonOperator.lt => true,
            _ => false
        };

        if (!valid)
            throw new ValidationAppException(InvalidOperatorExceptionCode, $"Недопустимый оператор: {op}");
    }

    private static void ValidateSetField(SetFieldAction action)
    {
        ValidateFieldExists(action.Field);
        ValidateValueType(action.Field, action.Value);
    }

    private static void ValidateNotification(CreateNotificationAction action)
    {
        if (string.IsNullOrWhiteSpace(action.Description))
            throw new ValidationAppException(InvalidValueExceptionCode, "Description не может быть пустым");
    }

    private static void ValidateCalendar(CreateCalendarEventAction action)
    {
        if (action.DurationMinutes <= 0)
            throw new ValidationAppException(InvalidValueExceptionCode, "DurationMinutes должен быть > 0");

        if (!string.IsNullOrEmpty(action.ExternalAccountId) && !Guid.TryParse(action.ExternalAccountId, out _)) 
            throw new ValidationAppException(InvalidValueExceptionCode, "ExternalAccountId должен быть валидным GUID или пустым");
    }
}
