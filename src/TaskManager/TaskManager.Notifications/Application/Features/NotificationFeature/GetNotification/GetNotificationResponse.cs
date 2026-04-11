using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

public sealed class GetNotificationResponse
{
    public Guid NotificationId { get; set; }
    public Guid? TaskId { get; set; }
    public int ServiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public NotificationStatus Status { get; set; }
}
