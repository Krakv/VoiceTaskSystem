namespace TaskManager.Application.Features.TaskItem.DTOs;

public sealed record UpdateTaskDto(
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    string DueDate,
    string Tags,
    string? ParentTaskId
    );
