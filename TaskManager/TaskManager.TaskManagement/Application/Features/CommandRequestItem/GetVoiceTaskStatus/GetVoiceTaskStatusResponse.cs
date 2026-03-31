namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusResponse(
    VoiceIntent Intent,
    IVoiceTaskPayload Payload
);
