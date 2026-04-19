using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid OwnerId,
    Guid TaskId,
    string? ProjectName,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    DateTimeOffset? DueDate,
    Guid? ParentTaskId
    ) : IRequest<Guid>;
