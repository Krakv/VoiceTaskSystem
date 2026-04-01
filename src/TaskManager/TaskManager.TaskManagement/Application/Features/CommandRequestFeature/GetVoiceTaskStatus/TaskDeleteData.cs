namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskDeleteData(
    bool ConfirmationRequired = false
    ) : IVoiceTaskPayload;
