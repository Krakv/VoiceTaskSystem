using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.DeleteNotification;

public record DeleteNotificationCommand(string NotificationId) : IRequest, INotificationAccessRequest;
