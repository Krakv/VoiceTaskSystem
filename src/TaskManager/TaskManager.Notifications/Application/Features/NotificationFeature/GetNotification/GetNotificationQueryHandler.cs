using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

public sealed class GetNotificationHandler(AppDbContext context) : IRequestHandler<GetNotificationQuery, GetNotificationResponse?>
{
    private readonly AppDbContext _context = context;

    public async Task<GetNotificationResponse?> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        return await _context.NotificationItem
            .AsNoTracking()
            .Where(x => x.NotificationId == Guid.Parse(request.NotificationId))
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
