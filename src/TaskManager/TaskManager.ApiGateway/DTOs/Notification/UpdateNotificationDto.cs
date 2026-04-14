namespace TaskManager.ApiGateway.DTOs.Notification;

public record UpdateNotificationDto(
    string Description,
    string ScheduledAt
);
