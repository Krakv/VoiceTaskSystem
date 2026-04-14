namespace TaskManager.ApiGateway.DTOs.Notification;

public sealed record CreateNotificationDto(
    int ServiceId,
    string Description,
    string ScheduledAt,
    string? TaskId
);
