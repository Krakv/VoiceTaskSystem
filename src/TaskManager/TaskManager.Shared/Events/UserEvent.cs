namespace TaskManager.Shared.Events;

public abstract class UserEvent : BaseEvent
{
    public abstract Guid UserId { get; init; }
}
