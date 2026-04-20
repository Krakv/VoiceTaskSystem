using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.DeleteNotification;

public record DeleteNotificationCommand(Guid OwnerId, Guid NotificationId) : IRequest, INotificationAccessRequest;
