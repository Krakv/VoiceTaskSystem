namespace TaskManager.RulesEngine.Domain.Actions;

public class CreateCalendarEventAction : RuleAction
{
    public int DurationMinutes { get; set; } = default!;
    public string Location { get; set; } = default!;

    public CreateCalendarEventAction()
    {
        Type = ActionType.CREATE_CALENDAR_EVENT;
    }
}
