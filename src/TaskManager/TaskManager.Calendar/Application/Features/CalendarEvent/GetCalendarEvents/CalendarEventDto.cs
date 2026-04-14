namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed record CalendarEventDto(
    Guid EventId,
    string Title,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Location,
    Guid? TaskId,
    Guid? ExternalAccountId
);
