namespace TaskManager.Application.Common.DTOs.Responses;

public class Error
{
    required public string Code { get; set; }
    required public string Message { get; set; }
    public Dictionary<string, string>? Fields { get; set; }
}
