using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotifications;

public class GetNotificationsQueryHandler(AppDbContext context, ICurrentUser user) : IRequestHandler<GetNotificationsQuery, List<GetNotificationResponse>>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<List<GetNotificationResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotificationItem
            .AsNoTracking()
            .Where(x => x.OwnerId == _user.UserId)
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
