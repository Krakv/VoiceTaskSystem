namespace TaskManager.ApiGateway.DTOs.CalendarEvent;

public sealed record UpdateCalendarEventDto(
    string CalendarEventId,
    string Title,
    string StartTime,
    string EndTime,
    string? Location,
    string? TaskId,
    string? ExternalAccountId
);
