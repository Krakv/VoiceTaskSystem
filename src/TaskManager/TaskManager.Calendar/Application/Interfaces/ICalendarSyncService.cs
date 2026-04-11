using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Calendar.Application.Interfaces;

public interface ICalendarSyncService
{
    Task SyncEventAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken);
    Task SyncDeleteEventAsync(Guid calendarEventId, Guid? externalAccountId, CancellationToken cancellationToken);

    Task SyncAllAsync(Guid externalAccountId, CancellationToken cancellationToken);
}
