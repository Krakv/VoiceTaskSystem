namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public record TaskDeleteData(
    bool ConfirmationRequired = false
    ) : IVoiceTaskPayload;
