using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;

public sealed record CreateNotificationCommand(
    Guid OwnerId,
    NotificationServiceType ServiceId,
    string Description,
    DateTimeOffset ScheduledAt,
    Guid? TaskId
) : IRequest<Guid>, ITaskAccessRequest;
