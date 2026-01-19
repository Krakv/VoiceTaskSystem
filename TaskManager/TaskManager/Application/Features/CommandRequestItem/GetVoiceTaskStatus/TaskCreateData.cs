namespace TaskManager.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public record TaskCreateData(
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTimeOffset DueDate,
    string Message,
    bool ConfirmationRequired = false
) : IVoiceTaskPayload;
