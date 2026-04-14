namespace TaskManager.ApiGateway.DTOs.CalendarEvent;

public sealed record CreateCalendarEventDto(
    string Title,
    string StartTime,
    string EndTime,
    string? Location,
    string? TaskId,
    string? ExternalAccountId
);