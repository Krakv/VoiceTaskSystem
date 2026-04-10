using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Shared.Domain.Entities;

[Table("CalendarEvent")]
public class CalendarEvent
{
    [Key]
    public Guid EventId { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    public Guid? TaskId { get; set; }
    [ForeignKey(nameof(TaskId))]
    public TaskItem? Task { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [Required]
    public DateTimeOffset StartTime { get; set; }

    [Required]
    public DateTimeOffset EndTime { get; set; }

    [MaxLength(255)]
    public string? Location { get; set; }

    [Required]
    [MaxLength(512)]
    public string? ExternalEventId { get; set; }

    public Guid? ExternalAccountId { get; set; }
    [ForeignKey(nameof(ExternalAccountId))]
    public ExternalCalendarAccount? ExternalCalendarAccount { get; set; }
}