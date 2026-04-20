namespace TaskManager.ApiGateway.DTOs.Rule;

public class CreateRuleDto
{
    public string RuleEvent { get; set; } = default!;
    public object? Conditions { get; set; }
    public IEnumerable<object> Actions { get; set; } = new List<object>();
    public bool IsActive { get; set; } = true;
}