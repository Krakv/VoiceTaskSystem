using MediatR;

namespace TaskManager.Shared.Events;

public abstract class BaseEvent : INotification
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public abstract Guid UserId { get; init; }
    public abstract string Event { get; init; }
}
