using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Notifications.Application.Services;

public class NotificationsProcessingService(AppDbContext dbContext, ILogger<NotificationsProcessingService> logger, IEmailService email) : INotificationsProcessingService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<NotificationsProcessingService> _logger = logger;
    private readonly IEmailService _email = email;
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
                var userEmail = n.Owner?.Email;

                if (!string.IsNullOrWhiteSpace(userEmail))
                {
                    await _email.SendAsync(userEmail, "Task Event", n.Description);
                    _logger.LogDebug("Sent {NotificationsId} notification", n.NotificationId);

                    n.Status = NotificationStatus.Sent;
                    n.SentAt = now;
                }
                else
                {
                    _logger.LogDebug("Email not available for user {UserId}", n.OwnerId);
                    _logger.LogDebug("Owner {OwnerName}?, {OwnerEmail}", n.Owner?.Name, userEmail);
                    n.Status = NotificationStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to process {Id}", n.NotificationId);

                n.Status = NotificationStatus.Failed;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
