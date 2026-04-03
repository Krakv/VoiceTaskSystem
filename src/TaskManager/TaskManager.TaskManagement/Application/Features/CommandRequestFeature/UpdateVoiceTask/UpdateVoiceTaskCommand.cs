using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

public sealed record UpdateVoiceTaskCommand(
    string CommandRequestId,
    string? TaskId,
    string? ProjectName,
    string? Title,
    string? Description,
    string? Status,
    string? DueDate,
    string? Priority,
    string? ParentTaskId
    ) : IRequest<UpdateVoiceTaskResponse>;
