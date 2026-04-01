namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskUpdateData(
    IReadOnlyDictionary<string, string> ProposedChanges,
    bool ConfirmationRequired = false
) : IVoiceTaskPayload;
