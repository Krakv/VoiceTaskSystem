using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotifications;

public class GetNotificationsQueryHandler(AppDbContext context) : IRequestHandler<GetNotificationsQuery, List<GetNotificationResponse>>
{
    private readonly AppDbContext _context = context;

    public async Task<List<GetNotificationResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {

        Guid ownerId = Guid.Parse(request.OwnerId);

        return await _context.NotificationItem
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId)
            .OrderBy(x => x.Status != NotificationStatus.Pending)
            .ThenBy(x => x.ScheduledAt)
            .Select(x => new GetNotificationResponse
            {
                NotificationId = x.NotificationId,
                TaskId = x.TaskId,
                ServiceId = x.ServiceId,
                Description = x.Description,
                ScheduledAt = x.ScheduledAt,
                SentAt = x.SentAt,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }
}
