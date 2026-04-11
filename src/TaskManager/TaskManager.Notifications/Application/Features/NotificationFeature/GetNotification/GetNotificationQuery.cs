using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

public sealed record GetNotificationQuery(string NotificationId) : IRequest<GetNotificationResponse?>, INotificationAccessRequest;
