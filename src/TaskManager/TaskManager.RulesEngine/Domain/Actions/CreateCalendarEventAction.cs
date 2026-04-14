namespace TaskManager.RulesEngine.Domain.Actions;

public class CreateCalendarEventAction : RuleAction
{
    public int DurationMinutes { get; set; }
    public int OffsetMinutes { get; set; } = 0;

    public string? Title { get; set; }
    public string? Location { get; set; }

    public string? ExternalAccountId { get; set; }

    public CreateCalendarEventAction()
    {
        Type = ActionType.CREATE_CALENDAR_EVENT;
    }
}
