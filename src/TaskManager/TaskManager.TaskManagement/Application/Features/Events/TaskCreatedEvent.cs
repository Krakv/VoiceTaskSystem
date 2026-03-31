using TaskManager.Shared.Events;

namespace TaskManager.TaskManagement.Application.Features.Events;

public sealed class TaskCreatedEvent(Guid TaskId, Guid OwnerId, string? Title) : BaseEvent
{
    public Guid TaskId { get; init; } = TaskId;
    public string? Title { get; init; } = Title;
    public override Guid UserId { get; init; } = OwnerId;
    public override string Event { get; init; } = nameof(TaskCreatedEvent);
}
