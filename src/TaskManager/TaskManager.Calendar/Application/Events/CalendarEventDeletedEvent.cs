using TaskManager.Shared.Events;

namespace TaskManager.Calendar.Application.Events;

public sealed class CalendarEventDeletedEvent(Guid ownerId, Guid calendarEventId, Guid? externalAccountId) : BaseEvent
{
    public Guid CalendarEventId { get; init; } = calendarEventId;
    public Guid? ExternalAccountId { get; init; } = externalAccountId;
    public override Guid UserId { get; init; } = ownerId;
    public override string Event { get; init; } = nameof(CalendarEventDeletedEvent);
}
