using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.ApiGateway.DTOs.Task;

public class UpdateTaskDto
{
    public string? ProjectName { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskItemPriority Priority { get; set; }
    public string? DueDate { get; set; }
    public string? ParentTaskId { get; set; }
}
