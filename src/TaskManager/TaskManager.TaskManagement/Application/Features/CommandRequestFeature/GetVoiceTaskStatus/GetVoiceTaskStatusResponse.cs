using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusResponse(
    CommandIntent? Intent,
    IVoiceTaskPayload? Payload
);
