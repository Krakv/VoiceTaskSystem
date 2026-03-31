using System.ComponentModel.DataAnnotations;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public sealed record TaskData(
    Guid TaskId,
    string ProjectName,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);

