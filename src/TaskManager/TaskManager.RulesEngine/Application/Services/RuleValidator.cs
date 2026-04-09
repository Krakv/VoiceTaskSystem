using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.RulesEngine.Domain.Conditions;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Services;

public class RuleValidator : IRuleValidator
{
    private static readonly HashSet<string> AllowedFields =
    [
        "title",
        "priority",
        "status",
        "projectName",
        "dueDate",
        "description",
        "parentTaskId"
    ];

    public void Validate(
        RuleEvent ruleEvent,
        ConditionGroup condition,
        IEnumerable<RuleAction> actions)
    {
        ValidateConditions(condition);
        ValidateActions(ruleEvent, actions);
    }

    private static void ValidateConditions(ConditionGroup condition)
    {
        if (condition == null)
            throw new ValidationAppException("INVALID_CONDITION", "Condition не может быть null");

        if (condition.Conditions == null || condition.Conditions.Count == 0)
            throw new ValidationAppException("INVALID_CONDITION", "Нет условий");

        foreach (var c in condition.Conditions)
        {
            if (string.IsNullOrWhiteSpace(c.Field))
                throw new ValidationAppException("INVALID_FIELD", "Field обязателен");

            if (!AllowedFields.Contains(c.Field))
                throw new ValidationAppException("INVALID_FIELD", $"Недопустимое поле: {c.Field}");
        }
    }

    private static void ValidateActions(RuleEvent ruleEvent, IEnumerable<RuleAction> actions)
    {
        if (actions == null || !actions.Any())
            throw new ValidationAppException("INVALID_ACTION", "Нет действий");

        foreach (var action in actions)
        {
            switch (action)
            {
                case SetFieldAction setField:
                    ValidateSetField(setField, ruleEvent);
                    break;

                case CreateNotificationAction notification:
                    ValidateNotification(notification);
                    break;

                case CreateCalendarEventAction calendar:
                    ValidateCalendar(calendar);
                    break;

                default:
                    throw new ValidationAppException("INVALID_ACTION", "Неизвестный тип действия");
            }
        }
    }

    private static void ValidateSetField(SetFieldAction action, RuleEvent ruleEvent)
    {
        if (string.IsNullOrWhiteSpace(action.Field))
            throw new ValidationAppException("INVALID_ACTION", "Field обязателен");

        if (!AllowedFields.Contains(action.Field))
            throw new ValidationAppException("INVALID_FIELD", $"Недопустимое поле: {action.Field}");

        if (action.Value == null)
            throw new ValidationAppException("INVALID_ACTION", "Value обязателен");

        // бизнес-ограничение
        if (ruleEvent == RuleEvent.TaskOverdue)
        {
            throw new ValidationAppException(
                "INVALID_ACTION",
                "Для события просрочки нельзя изменять поля");
        }
    }

    private static void ValidateNotification(CreateNotificationAction action)
    {
        if (string.IsNullOrWhiteSpace(action.Description))
            throw new ValidationAppException("INVALID_ACTION", "Описание обязательно");

        if (action.OffsetMinutes < 0)
            throw new ValidationAppException("INVALID_ACTION", "OffsetMinutes не может быть отрицательным");
    }

    private static void ValidateCalendar(CreateCalendarEventAction action)
    {
        if (action.DurationMinutes <= 0)
            throw new ValidationAppException("INVALID_ACTION", "DurationMinutes должен быть > 0");

        if (string.IsNullOrWhiteSpace(action.Location))
            throw new ValidationAppException("INVALID_ACTION", "Location обязателен");
    }
}
