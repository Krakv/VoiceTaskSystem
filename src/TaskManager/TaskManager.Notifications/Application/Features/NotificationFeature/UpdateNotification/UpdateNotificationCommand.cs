using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.UpdateNotification;

public record UpdateNotificationCommand(
    string NotificationId,
    string Description,
    string ScheduledAt,
    NotificationStatus Status
) : IRequest, INotificationAccessRequest;
