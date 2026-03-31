using TaskManager.TaskManagement.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskItem.GetTask;

public sealed record GetTaskResponse(
    string Title,
    string? ProjectName,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? ParentTaskId
    );
