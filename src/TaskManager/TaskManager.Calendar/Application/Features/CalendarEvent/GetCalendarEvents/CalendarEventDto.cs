namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed record CalendarEventDto(
    Guid EventId,
    string Title,
    string StartTime,
    string EndTime,
    string? Location,
    Guid? TaskId,
    Guid? ExternalAccountId
);
