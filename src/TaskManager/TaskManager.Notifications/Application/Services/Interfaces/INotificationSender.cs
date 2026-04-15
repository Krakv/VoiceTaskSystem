using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Notifications.Application.Services.Interfaces;

public interface INotificationSender
{
    NotificationServiceType ServiceId { get; }
    Task SendAsync(NotificationItem notification, CancellationToken cancellationToken = default);
}
