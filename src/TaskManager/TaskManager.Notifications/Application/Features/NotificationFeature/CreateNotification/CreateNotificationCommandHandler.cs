using MediatR;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using System.Globalization;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;

public sealed class CreateNotificationCommandHandler(AppDbContext context) : IRequestHandler<CreateNotificationCommand, Guid>
{
    private readonly AppDbContext _context = context;

    public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        Guid? taskId = string.IsNullOrWhiteSpace(request.TaskId) ? null : Guid.Parse(request.TaskId);
        Guid ownerId = Guid.Parse(request.OwnerId);

        var entity = new NotificationItem
        {
            TaskId = taskId,
            OwnerId = ownerId,
            ServiceId = request.ServiceId,
            Description = request.Description,
            ScheduledAt = DateTimeOffset.Parse(request.ScheduledAt, CultureInfo.InvariantCulture)
        };

        _context.NotificationItem.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.NotificationId;
    }
}
