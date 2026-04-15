using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Events;

namespace TaskManager.Calendar.Application.Events;

public sealed class CalendarEventUpdatedEvent(Guid ownerId, CalendarEvent calendarEvent) : UserEvent
{
    public CalendarEvent CalendarEvent { get; init; } = calendarEvent;
    public override Guid UserId { get; init; } = ownerId;
    public override string Event { get; init; } = nameof(CalendarEventUpdatedEvent);
}
