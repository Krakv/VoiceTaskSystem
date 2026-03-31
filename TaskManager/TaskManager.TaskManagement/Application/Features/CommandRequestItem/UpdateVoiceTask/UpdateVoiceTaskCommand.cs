using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.UpdateVoiceTask;

public sealed record UpdateVoiceTaskCommand(
    string CommandRequestId,
    string? ProjectName,
    string? Title,
    string? Description,
    string? Status,
    string? DueDate,
    string? Priority
    ) : IRequest<UpdateVoiceTaskResponse>;
