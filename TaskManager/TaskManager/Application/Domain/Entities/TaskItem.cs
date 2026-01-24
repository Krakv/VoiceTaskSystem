using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("Task")]
public class TaskItem
{
    [Key]
    public Guid TaskId { get; set; } = Guid.NewGuid();
    public required Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string Priority { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    public Guid? ParentTaskId { get; set; }
    [ForeignKey(nameof(ParentTaskId))]
    public TaskItem? Parent { get; set; }

    public ICollection<TaskItem> Children { get; set; } = [];

    public ICollection<CommandRequestItem> CommandRequests { get; set; } = [];
}
