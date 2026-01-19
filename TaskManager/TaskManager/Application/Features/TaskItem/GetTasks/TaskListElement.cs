namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed record TaskListElement(
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
