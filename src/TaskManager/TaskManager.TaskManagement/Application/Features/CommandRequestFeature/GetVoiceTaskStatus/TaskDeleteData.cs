namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskDeleteData(
    List<Guid> TaskIds,
    bool ConfirmationRequired = false
    ) : IVoiceTaskPayload;
