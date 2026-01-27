using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("Notification")]
public class NotificationItem
{
    [Key]
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    public required Guid TaskId { get; set; }
    [ForeignKey(nameof(TaskId))]
    public required TaskItem Task { get; set; }

    public required Guid ServiceId { get; set; }
    [ForeignKey(nameof(ServiceId))]
    public required ServiceItem Service { get; set; }

    public string Description { get; set; } = string.Empty;
    public required DateTimeOffset ScheduledAt { get; set; }
    public DateTimeOffset? SentAt { get; set;}
    public string Status { get; set; } = "Pending";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
