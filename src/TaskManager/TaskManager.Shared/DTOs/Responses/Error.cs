namespace TaskManager.Shared.DTOs.Responses;

public class Error
{
    required public string Code { get; set; }
    required public string Message { get; set; }
    public IDictionary<string, string>? Fields { get; set; }
}
