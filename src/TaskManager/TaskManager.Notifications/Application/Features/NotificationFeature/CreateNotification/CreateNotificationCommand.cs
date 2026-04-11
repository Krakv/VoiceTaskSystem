using MediatR;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Application.Features.NotificationFeature.CreateNotification;

public sealed record CreateNotificationCommand(
    int ServiceId,
    string Description,
    string ScheduledAt,
    string? TaskId
) : IRequest<Guid>, ITaskAccessRequest;
