using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

public sealed record UpdateVoiceTaskCommand(
    Guid OwnerId,
    Guid CommandRequestId,
    Guid? TaskId,
    string? ProjectName,
    string? Title,
    string? Description,
    TaskItemStatus? Status,
    string? DueDate,
    TaskItemPriority? Priority,
    string? ParentTaskId
    ) : IRequest<UpdateVoiceTaskResponse>;
