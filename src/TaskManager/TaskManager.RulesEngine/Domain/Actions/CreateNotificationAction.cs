using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.RulesEngine.Domain.Actions;

public class CreateNotificationAction : RuleAction
{
    public string Description { get; set; } = default!;
    public int OffsetMinutes { get; set; } = default!;
    public NotificationServiceType ServiceId { get; set; } = NotificationServiceType.Email;

    public CreateNotificationAction()
    {
        Type = ActionType.CREATE_NOTIFICATION;
    }
}
