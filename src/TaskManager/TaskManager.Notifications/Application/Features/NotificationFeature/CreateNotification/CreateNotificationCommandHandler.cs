using MediatR;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;

public sealed class CreateNotificationCommandHandler(AppDbContext context) : IRequestHandler<CreateNotificationCommand, Guid>
{
    private readonly AppDbContext _context = context;

    public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = new NotificationItem
        {
            TaskId = request.TaskId,
            OwnerId = request.OwnerId,
            ServiceId = request.ServiceId,
            Description = request.Description,
            ScheduledAt = request.ScheduledAt,
        };

        _context.NotificationItem.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.NotificationId;
    }
}
