using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;

public sealed record UpdateCalendarEventCommand(
    string Id,
    string StartTime,
    string EndTime,
    string? Location,
    string? TaskId,
    string? ExternalAccountId
) : IRequest;
