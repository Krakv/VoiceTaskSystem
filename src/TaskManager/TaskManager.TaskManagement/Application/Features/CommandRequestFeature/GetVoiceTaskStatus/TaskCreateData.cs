using TaskManager.Shared.Domain.Entities;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskCreateData(
    string? ProjectName,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    DateTimeOffset? DueDate,
    string Message,
    Guid? ParentTaskId,
    TaskShortInfoDto? ParentTask,
    bool ConfirmationRequired = true
) : IVoiceTaskPayload;
