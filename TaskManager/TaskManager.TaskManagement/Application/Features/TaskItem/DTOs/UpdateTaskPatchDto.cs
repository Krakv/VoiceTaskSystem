namespace TaskManager.Application.Features.TaskItem.DTOs;

public sealed record UpdateTaskPatchDto(
    string? ProjectName = null,
    string? Title = null,
    string? Description = null,
    string? Status = null,
    string? Priority = null,
    string? DueDate = null,
    string? Tags = null,
    string? ParentTaskId = null
);
