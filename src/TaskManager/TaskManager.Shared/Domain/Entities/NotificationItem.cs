using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.Domain.Entities;

[Table("Notification")]
public class NotificationItem
{
    [Key]
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    public Guid? TaskId { get; set; }
    [ForeignKey(nameof(TaskId))]
    public TaskItem? Task { get; set; }

    public NotificationServiceType ServiceId { get; set; }

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset ScheduledAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    [Required]
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}