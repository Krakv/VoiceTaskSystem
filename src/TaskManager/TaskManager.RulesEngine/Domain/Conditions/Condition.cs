namespace TaskManager.RulesEngine.Domain.Conditions;

public class Condition
{
    public string Field { get; set; } = default!;
    public ComparisonOperator Operator { get; set; } = default!;
    public string Value { get; set; } = default!;
}
