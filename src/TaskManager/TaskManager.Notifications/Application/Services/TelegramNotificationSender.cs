using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.Notifications.Application.Services;

public class TelegramNotificationSender(IBotService botService) : INotificationSender
{
    private readonly IBotService _botService = botService;

    public NotificationServiceType ServiceId => NotificationServiceType.Telegram;

    public async Task SendAsync(NotificationItem notification, CancellationToken cancellationToken = default)
    {
        if (notification.Owner.TelegramChatId != null)
        {
            await _botService.SendCommand(notification.Owner.TelegramChatId.Value, notification.Description, cancellationToken);
        }
    }
}
