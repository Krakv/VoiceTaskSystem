using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.GetNotification;

public sealed record GetNotificationQuery(Guid OwnerId, Guid NotificationId) : IRequest<GetNotificationResponse>, INotificationAccessRequest;
