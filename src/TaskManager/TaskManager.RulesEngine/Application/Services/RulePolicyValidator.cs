using TaskManager.RulesEngine.Application.Interfaces;
using TaskManager.RulesEngine.Domain.Actions;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Exceptions;

namespace TaskManager.RulesEngine.Application.Services;

public class RulePolicyValidator : IRulePolicyValidator
{
    private const string PolicyViolationException = "POLICY_VIOLATION";

    public void Validate(RuleEvent ruleEvent, IEnumerable<RuleAction> actions)
    {
        foreach (var action in actions)
        {
            ValidateAction(ruleEvent, action);
        }
    }

    private static void ValidateAction(RuleEvent ruleEvent, RuleAction action)
    {
        switch (action)
        {
            case SetFieldAction _:
                ValidateSetField(ruleEvent);
                break;

            case CreateNotificationAction _:
                ValidateNotification();
                break;

            case CreateCalendarEventAction calendar:
                ValidateCalendar(calendar);
                break;
        }
    }

    private static void ValidateSetField(RuleEvent ruleEvent)
    {
        if (ruleEvent == RuleEvent.TaskOverdue)
        {
            throw new ValidationAppException(
                PolicyViolationException,
                "Нельзя изменять поля при просрочке задачи");
        }
    }

    private static void ValidateNotification()
    {
        // No specific validation logic for notifications at the moment
    }

    private static void ValidateCalendar(CreateCalendarEventAction action)
    {
        if (action.DurationMinutes > 1440)
        {
            throw new ValidationAppException(
                PolicyViolationException,
                "Событие не может длиться больше суток");
        }
    }
}
