using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.TaskManagement.Domain.Entities.Enum;
namespace TaskManager.TaskManagement.Domain.Entities;

[Table("Task")]
public class TaskItem
{
    [Key]
    public Guid TaskId { get; set; } = Guid.NewGuid();

    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    [MaxLength(100)]
    public string? ProjectName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    [Required]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.New;

    [Required]
    public TaskItemPriority Priority { get; set; } = TaskItemPriority.Low;

    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    public Guid? ParentTaskId { get; set; }
    [ForeignKey(nameof(ParentTaskId))]
    public TaskItem? Parent { get; set; }
    public ICollection<TaskItem> Children { get; set; } = [];
    public ICollection<CommandRequestItem> CommandRequests { get; set; } = [];
}