using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

public sealed class GetNotificationHandler(AppDbContext context) : IRequestHandler<GetNotificationQuery, GetNotificationResponse>
{
    private readonly AppDbContext _context = context;

    public async Task<GetNotificationResponse> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var notification = await _context.NotificationItem
            .AsNoTracking()
            .Where(x => x.NotificationId == request.NotificationId && x.OwnerId == request.OwnerId)
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

        return notification ?? throw new ValidationAppException("NOT_FOUND", "Уведомление не найдено");
    }
}
