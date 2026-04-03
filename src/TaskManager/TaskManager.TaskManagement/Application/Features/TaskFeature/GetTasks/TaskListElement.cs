using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

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
