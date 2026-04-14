using MediatR;
using TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotifications;

public record GetNotificationsQuery(string OwnerId) : IRequest<List<GetNotificationResponse>>;
