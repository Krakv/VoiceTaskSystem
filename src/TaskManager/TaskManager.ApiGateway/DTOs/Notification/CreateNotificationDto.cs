using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.ApiGateway.DTOs.Notification;

public sealed record CreateNotificationDto(
    NotificationServiceType ServiceId,
    string Description,
    string ScheduledAt,
    string? TaskId
);
