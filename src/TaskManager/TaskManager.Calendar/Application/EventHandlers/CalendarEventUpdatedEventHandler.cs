using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Calendar.Application.Events;
using TaskManager.Calendar.Application.Interfaces;
using TaskManager.Repository.Context;
using TaskManager.Shared.EventHandlers;

namespace TaskManager.Calendar.Application.EventHandlers;

public sealed class CalendarEventUpdatedEventHandler(
    ICalendarSyncService syncService,
    ILogger<BaseEventHandler<CalendarEventUpdatedEvent>> logger)
    : BaseEventHandler<CalendarEventUpdatedEvent>(logger)
{
    private readonly ICalendarSyncService _syncService = syncService;

    protected override async Task Process(CalendarEventUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.CalendarEvent is null) return;

        await _syncService.SyncEventAsync(notification.CalendarEvent, cancellationToken);
    }
}
