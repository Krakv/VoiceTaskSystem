namespace TaskManager.Application.Features.TaskItem.CreateTask;

public sealed record CreateTaskResponse(
    string TaskId,
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
    );