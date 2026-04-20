using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;

public class UpdateNotificationCommandHandler(AppDbContext context) : IRequestHandler<UpdateNotificationCommand>
{
    private readonly AppDbContext _context = context;

    public async Task Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NotificationItem
            .FirstOrDefaultAsync(x => x.NotificationId == request.NotificationId
            && x.OwnerId == request.OwnerId, cancellationToken) 
            ?? throw new ValidationAppException("NOT_FOUND", "Уведомление не найдено");

        entity.Description = request.Description;
        entity.ScheduledAt = request.ScheduledAt;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
