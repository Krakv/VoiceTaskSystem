namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed record TaskListElement(
    Guid TaskId,
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    string Tags,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? ParentTaskId
    );
