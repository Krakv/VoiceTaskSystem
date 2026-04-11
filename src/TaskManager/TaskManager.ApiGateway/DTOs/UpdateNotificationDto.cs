using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.ApiGateway.DTOs;

public record UpdateNotificationDto(
    string Description,
    string ScheduledAt,
    NotificationStatus NotificationStatus
);
