using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TaskManager.Application.Domain.Entities.Enum;
namespace TaskManager.Application.Domain.Entities;

[Table("Notification")]
public class NotificationItem
{
    [Key]
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    public Guid TaskId { get; set; }
    [ForeignKey(nameof(TaskId))]
    public TaskItem Task { get; set; } = null!;

    public int ServiceId { get; set; }
    [ForeignKey(nameof(ServiceId))]
    public ServiceItem Service { get; set; } = null!;

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset ScheduledAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    [Required]
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}