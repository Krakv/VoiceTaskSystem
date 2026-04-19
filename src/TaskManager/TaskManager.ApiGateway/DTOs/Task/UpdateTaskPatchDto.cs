namespace TaskManager.ApiGateway.DTOs.Task;

public class UpdateTaskPatchDto
{
    public string? ProjectName { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public string? Status { get; set; }
    public string? Priority { get; set; }

    public string? DueDate { get; set; }
    public string? ParentTaskId { get; set; }
}
