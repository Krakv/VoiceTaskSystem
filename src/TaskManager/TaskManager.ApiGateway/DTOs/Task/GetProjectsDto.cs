namespace TaskManager.ApiGateway.DTOs.Task;

public class GetProjectsDto
{
    public string Project { get; set; } = null!;
    public string Limit { get; set; } = "20";
    public string Page { get; set; } = "0";
}
