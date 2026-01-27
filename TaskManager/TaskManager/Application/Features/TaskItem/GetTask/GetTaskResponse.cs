namespace TaskManager.Application.Features.TaskItem.GetTask;

public sealed record GetTaskResponse(
    string Title,
    string? ProjectName,
    string? Description,
    string Status,
    string Priority,
    string? Tags,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? ParentTaskId
    );
