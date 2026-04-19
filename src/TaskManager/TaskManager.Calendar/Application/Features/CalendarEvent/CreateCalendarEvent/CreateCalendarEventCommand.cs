using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;

public sealed record CreateCalendarEventCommand(
    Guid OwnerId,
    string Title,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string? Location,
    Guid? TaskId,
    Guid? ExternalAccountId
) : IRequest<Guid>;