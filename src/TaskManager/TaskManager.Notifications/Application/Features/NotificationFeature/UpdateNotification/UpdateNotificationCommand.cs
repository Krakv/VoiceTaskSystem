using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;

public record UpdateNotificationCommand(
    Guid OwnerId,
    Guid NotificationId,
    string Description,
    DateTimeOffset ScheduledAt
) : IRequest, INotificationAccessRequest;
