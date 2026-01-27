namespace TaskManager.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public record TaskUpdateData(
    IReadOnlyDictionary<string, string> ProposedChanges,
    bool ConfirmationRequired = false
) : IVoiceTaskPayload;
