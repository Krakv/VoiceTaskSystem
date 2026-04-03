namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskCreateData(
    string? ProjectName,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTimeOffset? DueDate,
    string Message,
    Guid? ParentTaskId,
    bool ConfirmationRequired = true
) : IVoiceTaskPayload;
