using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleSchemaValidator : IRuleSchemaValidator
{
    private const string SchemaInvalidCode = "SCHEMA_INVALID";

    public void ValidateStructure(ConditionGroup? condition, IEnumerable<RuleAction> actions)
    {
        ValidateActions(actions);
        ValidateConditions(condition);
    }

    private static void ValidateActions(IEnumerable<RuleAction> actions)
    {
        if (actions == null)
            throw new ValidationAppException(SchemaInvalidCode, "Actions не могут быть null");

        if (!actions.Any())
            throw new ValidationAppException(SchemaInvalidCode, "Должно быть хотя бы одно действие");

        foreach (var action in actions)
        {
            if (action == null)
                throw new ValidationAppException(SchemaInvalidCode, "Action не может быть null");

            ValidateActionShape(action);
        }
    }

    private static void ValidateActionShape(RuleAction action)
    {
        switch (action)
        {
            case SetFieldAction set:
                if (string.IsNullOrWhiteSpace(set.Field))
                    throw new ValidationAppException(SchemaInvalidCode, "Field обязателен");

                if (string.IsNullOrWhiteSpace(set.Value))
                    throw new ValidationAppException(SchemaInvalidCode, "Value обязателен");
                break;

            case CreateNotificationAction notif:
                if (notif.Description == null)
                    throw new ValidationAppException(SchemaInvalidCode, "Description обязателен");
                break;

            case CreateCalendarEventAction calendar:
                if (calendar.DurationMinutes == 0)
                    throw new ValidationAppException(SchemaInvalidCode, "DurationMinutes обязателен");
                break;

            default:
                throw new ValidationAppException(SchemaInvalidCode, "Неизвестный тип действия");
        }
    }

    private static void ValidateConditions(ConditionGroup? condition)
    {
        if (condition == null)
            return;

        if (condition.Conditions == null)
            throw new ValidationAppException(SchemaInvalidCode, "Conditions не могут быть null");

        foreach (var c in condition.Conditions)
        {
            if (c == null)
                throw new ValidationAppException(SchemaInvalidCode, "Condition не может быть null");

            if (string.IsNullOrWhiteSpace(c.Field))
                throw new ValidationAppException(SchemaInvalidCode, "Field обязателен");

            if (string.IsNullOrWhiteSpace(c.Value))
                throw new ValidationAppException(SchemaInvalidCode, "Value обязателен");
        }
    }
}

