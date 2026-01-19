namespace TaskManager.Application.Common.DTOs.Responses;

public class Meta
{
    required public string RequestId { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}
