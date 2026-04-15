using MediatR;

namespace TaskManager.Shared.Events;

public abstract class BaseEvent : INotification
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public abstract string Event { get; init; }
}
