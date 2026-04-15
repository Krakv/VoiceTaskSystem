using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Notifications.Application.Services;

public class EmailNotificationSender(IEmailService email) : INotificationSender
{
    private readonly IEmailService _email = email;

    public NotificationServiceType ServiceId => NotificationServiceType.Email;

    public async Task SendAsync(NotificationItem notification, CancellationToken cancellationToken = default)
    {
        var emailAddress = notification.Owner?.Email;
        var userEmailConfirmed = notification.Owner?.EmailConfirmed;

        if (!string.IsNullOrWhiteSpace(emailAddress) && (userEmailConfirmed ?? false))
        {
            await _email.SendAsync(emailAddress, "Task Event", notification.Description);
        }
    }
}
