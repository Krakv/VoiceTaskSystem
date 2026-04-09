namespace TaskManager.RulesEngine.Domain.Actions;

public class SetFieldAction : RuleAction
{
    public string Field { get; set; } = default!;
    public string Value { get; set; } = default!;

    public SetFieldAction()
    {
        Type = ActionType.SET_FIELD;
    }
}
