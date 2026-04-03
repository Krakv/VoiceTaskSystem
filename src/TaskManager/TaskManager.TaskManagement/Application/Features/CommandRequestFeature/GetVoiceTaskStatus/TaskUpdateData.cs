namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskUpdateData(
    List<Guid> TaskIds,
    string? ProjectName,
    string? Description,
    string? Status,
    string? Priority,
    DateTimeOffset? DueDate,
    Guid? ParentTaskId,
    bool ConfirmationRequired = true
) : IVoiceTaskPayload;
