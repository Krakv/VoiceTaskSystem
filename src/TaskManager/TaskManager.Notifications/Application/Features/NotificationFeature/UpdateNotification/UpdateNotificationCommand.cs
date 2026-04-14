using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;

public record UpdateNotificationCommand(
    string OwnerId,
    string NotificationId,
    string Description,
    string ScheduledAt
) : IRequest, INotificationAccessRequest;
