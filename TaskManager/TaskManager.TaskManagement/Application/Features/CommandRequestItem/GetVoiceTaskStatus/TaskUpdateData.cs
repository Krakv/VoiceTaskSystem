namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public record TaskUpdateData(
    IReadOnlyDictionary<string, string> ProposedChanges,
    bool ConfirmationRequired = false
) : IVoiceTaskPayload;
