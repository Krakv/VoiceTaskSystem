namespace TaskManager.RulesEngine.Domain.Conditions;

public class ConditionGroup()
{
    public LogicalOperator Operator { get; set; } = default;
    public List<Condition> Conditions { get; set; } = new();
}
