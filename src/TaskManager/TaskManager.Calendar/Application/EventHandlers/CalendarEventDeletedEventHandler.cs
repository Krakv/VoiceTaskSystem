using Microsoft.Extensions.Logging;
using TaskManager.Calendar.Application.Events;
using TaskManager.Calendar.Application.Interfaces;
using TaskManager.Shared.EventHandlers;

namespace TaskManager.Calendar.Application.EventHandlers;

public sealed class CalendarEventDeletedEventHandler(
    ICalendarSyncService syncService,
    ILogger<BaseEventHandler<CalendarEventDeletedEvent>> logger)
    : BaseEventHandler<CalendarEventDeletedEvent>(logger)
{
    private readonly ICalendarSyncService _syncService = syncService;

    protected override async Task Process(CalendarEventDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _syncService.SyncDeleteEventAsync(notification.CalendarEventId, notification.ExternalAccountId, cancellationToken);
    }
}
