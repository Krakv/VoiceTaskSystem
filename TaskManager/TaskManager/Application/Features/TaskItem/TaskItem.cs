using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.Features.TaskItem;

public class TaskItem
{
    [Key]
    public Guid TaskId { get; set; } = Guid.NewGuid();
    public long OwnerId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}
