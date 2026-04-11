using MediatR;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities;
using System.Globalization;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;

public sealed class CreateNotificationCommandHandler(AppDbContext context, ICurrentUser user) : IRequestHandler<CreateNotificationCommand, Guid>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<Guid> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var taskId = Guid.Parse(request.TaskId);

        var entity = new NotificationItem
        {
            TaskId = taskId,
            OwnerId = _user.UserId,
            ServiceId = request.ServiceId,
            Description = request.Description,
            ScheduledAt = DateTimeOffset.Parse(request.ScheduledAt, CultureInfo.InvariantCulture)
        };

        _context.NotificationItem.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.NotificationId;
    }
}
