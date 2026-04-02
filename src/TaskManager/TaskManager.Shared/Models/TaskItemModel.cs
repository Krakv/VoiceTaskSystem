using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.Models;

public class TaskItemModel
{
    public string? ProjectName { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public TaskItemStatus? Status { get; set; }

    public TaskItemPriority? Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public string? ParentTaskName { get; set; }
}
