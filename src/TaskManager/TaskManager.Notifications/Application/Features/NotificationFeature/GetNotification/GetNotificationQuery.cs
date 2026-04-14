using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

public sealed record GetNotificationQuery(string OwnerId, string NotificationId) : IRequest<GetNotificationResponse?>, INotificationAccessRequest;
