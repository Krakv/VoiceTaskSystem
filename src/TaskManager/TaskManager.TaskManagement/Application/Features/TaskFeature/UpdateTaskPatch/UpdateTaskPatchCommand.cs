using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

public sealed record UpdateTaskPatchCommand(
    Guid OwnerId,
    Guid TaskId,
    string? ProjectName = null,
    string? Title = null,
    string? Description = null,
    TaskItemStatus? Status = null,
    TaskItemPriority? Priority = null,
    string? DueDate = null,
    string? ParentTaskId = null
) : IRequest<Guid>;
