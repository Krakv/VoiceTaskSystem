using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;

public sealed record UpdateCalendarEventCommand(
    Guid OwnerId,
    Guid CalendarEventId,
    string Title,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Location,
    Guid? TaskId,
    Guid? ExternalAccountId
) : IRequest;
