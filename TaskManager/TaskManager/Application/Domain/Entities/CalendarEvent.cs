using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("CalendarEvent")]
public class CalendarEvent
{
    [Key]
    public Guid EventId { get; set; } = Guid.NewGuid();

    public Guid TaskId { get; set; }
    [ForeignKey(nameof(TaskId))]
    public TaskItem? Task { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public required string Location { get; set; }
    public required string ExternalEventId { get; set; }

    public required string ExternalAccountId { get; set; }
    [ForeignKey(nameof(ExternalAccountId))]
    public ExternalCalendarAccount? ExternalCalendarAccount { get; set; }
}
