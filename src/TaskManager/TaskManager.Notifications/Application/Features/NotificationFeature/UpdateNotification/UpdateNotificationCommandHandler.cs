using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskManager.Repository.Context;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;

public class UpdateNotificationCommandHandler(AppDbContext context) : IRequestHandler<UpdateNotificationCommand>
{
    private readonly AppDbContext _context = context;

    public async Task Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationId = Guid.Parse(request.NotificationId);

        var entity = await _context.NotificationItem
            .FirstOrDefaultAsync(x => x.NotificationId == notificationId, cancellationToken);

        if (entity == null) return;

        entity.Description = request.Description;
        entity.ScheduledAt = DateTimeOffset.Parse(request.ScheduledAt, CultureInfo.InvariantCulture);
        entity.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
