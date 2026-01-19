namespace TaskManager.Application.Features.TaskItem.UpdateTask;

public sealed record UpdateTaskResponse(
    string? ProjectName,
    string? Title,
    string? Description,
    string? Status,
    string? Priority,
    DateTimeOffset? DueDate
    );
