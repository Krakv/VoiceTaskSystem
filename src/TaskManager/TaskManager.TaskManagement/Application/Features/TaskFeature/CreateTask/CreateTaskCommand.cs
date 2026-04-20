using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

public sealed record CreateTaskCommand(
    Guid OwnerId,
    string? ProjectName,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    DateTimeOffset? DueDate,
    Guid? ParentTaskId
    ) : IRequest<Guid>;
