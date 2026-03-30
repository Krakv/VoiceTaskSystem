using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManager.Application.Domain.Entities;

[Table("ExternalCalendarAccount")]
public class ExternalCalendarAccount
{
    [Key]
    public Guid ExternalCalendarAccountId { get; set; } = Guid.NewGuid();

    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    [Required]
    public CalendarProvider Provider { get; set; }

    [Required]
    [MaxLength(2048)]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset ExpiresAt { get; set; }

    public ICollection<CalendarEvent> CalendarEvents { get; set; } = [];
}

public enum CalendarProvider
{
    Yandex = 1,
    Google = 2,
    Outlook = 3
}