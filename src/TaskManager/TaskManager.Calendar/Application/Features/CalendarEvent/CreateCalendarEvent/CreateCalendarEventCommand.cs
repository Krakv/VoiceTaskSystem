using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;

public sealed record CreateCalendarEventCommand(
    string OwnerId,
    string Title,
    string StartTime,
    string EndTime,
    string? Location,
    string? TaskId,
    string? ExternalAccountId
) : IRequest<Guid>;