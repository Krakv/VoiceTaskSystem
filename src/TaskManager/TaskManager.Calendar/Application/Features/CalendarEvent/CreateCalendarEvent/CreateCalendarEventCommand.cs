using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;

public sealed record CreateCalendarEventCommand(
    string StartTime,
    string EndTime,
    string? Location,
    string? TaskId
) : IRequest<Guid>;