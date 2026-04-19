namespace TaskManager.ApiGateway.DTOs.Task;

public class GetTasksDto
{
    public string? Query { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public string? Limit { get; set; }
    public string? Page { get; set; }
}
