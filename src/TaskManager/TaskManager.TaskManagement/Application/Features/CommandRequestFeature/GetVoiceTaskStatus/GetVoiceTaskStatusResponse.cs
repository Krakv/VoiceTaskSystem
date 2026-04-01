namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusResponse(
    VoiceIntent Intent,
    IVoiceTaskPayload Payload
);
