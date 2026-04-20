using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.DeleteNotification;

public class DeleteNotificationCommandHandler(AppDbContext context) : IRequestHandler<DeleteNotificationCommand>
{
    private readonly AppDbContext _context = context;

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NotificationItem
            .FirstOrDefaultAsync(
                x => x.NotificationId == request.NotificationId
                  && x.OwnerId == request.OwnerId,
                cancellationToken
            ) 
            ?? throw new ValidationAppException("NOT_FOUND", "Уведомление не найдено");
        _context.NotificationItem.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

    }
}
