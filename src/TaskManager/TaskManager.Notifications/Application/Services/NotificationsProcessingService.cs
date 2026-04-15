using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Notifications.Application.Services;

public class NotificationsProcessingService : INotificationsProcessingService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<NotificationsProcessingService> _logger;
    private readonly Dictionary<NotificationServiceType, INotificationSender> _sendersMap;

    public NotificationsProcessingService(
        AppDbContext dbContext,
        ILogger<NotificationsProcessingService> logger,
        IEnumerable<INotificationSender> senders)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sendersMap = senders.ToDictionary(s => s.ServiceId);
    }

    public async Task ProcessNotificationsAsync()
    {
        var now = DateTimeOffset.UtcNow;

        var notifications = await _dbContext.NotificationItem
            .Include(x => x.Owner)
            .Where(x => x.Status == NotificationStatus.Pending &&
                        x.ScheduledAt <= now)
            .ToListAsync();

        foreach (var n in notifications)
        {
            n.Status = NotificationStatus.Processing;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("Started to process {NotificationsCount} notifications", notifications.Count);

        foreach (var n in notifications)
        {
            try
            {
                if (!_sendersMap.TryGetValue(n.ServiceId, out var sender) || sender == null)
                {
                    _logger.LogWarning("No sender found for service {ServiceId}", n.ServiceId);
                    n.Status = NotificationStatus.Failed;
                    continue;
                }

                await sender.SendAsync(n);

                _logger.LogDebug("Sent {NotificationId} via service {ServiceId}", n.NotificationId, n.ServiceId);

                n.Status = NotificationStatus.Sent;
                n.SentAt = now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unable to process notification {NotificationId} for service {ServiceId}",
                    n.NotificationId, n.ServiceId);

                n.Status = NotificationStatus.Failed;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
