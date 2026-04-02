namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskUpdateData(
    List<Guid> TaskIds,
    string? Description,
    string? Status,
    string? Priority,
    DateTimeOffset? DueDate,
    Guid? ParentTaskId,
    bool ConfirmationRequired = true
) : IVoiceTaskPayload;
