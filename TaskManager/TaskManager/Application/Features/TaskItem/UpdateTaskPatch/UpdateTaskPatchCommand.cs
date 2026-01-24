using MediatR;

namespace TaskManager.Application.Features.TaskItem.UpdateTaskPatch;

public sealed record UpdateTaskPatchCommand(
    string TaskId,
    string? ProjectName = null,
    string? Title = null,
    string? Description = null,
    string? Status = null,
    string? Priority = null,
    string? DueDate = null,
    string? Tags = null,
    string? ParentTaskId = null
) : IRequest<string>;
