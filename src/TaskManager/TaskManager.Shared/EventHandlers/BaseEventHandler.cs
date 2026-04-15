using MediatR;
using TaskManager.Shared.Events;
using Microsoft.Extensions.Logging;

namespace TaskManager.Shared.EventHandlers;

public abstract class BaseEventHandler<TEvent>(ILogger<BaseEventHandler<TEvent>> logger) : INotificationHandler<TEvent> where TEvent : BaseEvent
{
    private readonly ILogger<BaseEventHandler<TEvent>> _logger = logger;

    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        Log(notification);
        await Process(notification, cancellationToken);
    }

    protected virtual void Log(TEvent notification)
    {
        _logger.LogInformation("Event {EventId} | {Event} ", notification.EventId, notification.Event);
    }

    protected abstract Task Process(TEvent notification, CancellationToken cancellationToken);
}
