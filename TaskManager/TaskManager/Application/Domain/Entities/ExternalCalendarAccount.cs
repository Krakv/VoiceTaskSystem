using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("ExternalCalendarAccount")]
public class ExternalCalendarAccount
{
    public Guid ExternalCalendarAccountId { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }

    public required string Provider {  get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}
