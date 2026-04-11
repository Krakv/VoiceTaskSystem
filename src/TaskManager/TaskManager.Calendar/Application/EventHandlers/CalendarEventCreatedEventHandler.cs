using Microsoft.Extensions.Logging;
using TaskManager.Calendar.Application.Events;
using TaskManager.Calendar.Application.Interfaces;
using TaskManager.Shared.EventHandlers;

namespace TaskManager.Calendar.Application.EventHandlers;

public sealed class CalendarEventCreatedEventHandler(
    ICalendarSyncService syncService,
    ILogger<BaseEventHandler<CalendarEventCreatedEvent>> logger)
    : BaseEventHandler<CalendarEventCreatedEvent>(logger)
{
    private readonly ICalendarSyncService _syncService = syncService;

    protected override async Task Process(CalendarEventCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CalendarEvent is null) return;

        await _syncService.SyncEventAsync(notification.CalendarEvent, cancellationToken);
    }
}
