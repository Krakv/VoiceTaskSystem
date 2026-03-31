using TaskManager.Application.Domain.Entities.Enum;

namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed record TaskListElement(
    Guid TaskId,
    string? ProjectName,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? ParentTaskId
    );
