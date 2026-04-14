namespace TaskManager.RulesEngine.Domain.Actions;

public class CreateNotificationAction : RuleAction
{
    public string Description { get; set; } = default!;
    public int OffsetMinutes { get; set; } = default!;
    public int ServiceId { get; set; } = 2;

    public CreateNotificationAction()
    {
        Type = ActionType.CREATE_NOTIFICATION;
    }
}
