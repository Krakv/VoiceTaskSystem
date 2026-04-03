namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed record TaskData(
    Guid TaskId,
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    Guid ParentTaskId,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

