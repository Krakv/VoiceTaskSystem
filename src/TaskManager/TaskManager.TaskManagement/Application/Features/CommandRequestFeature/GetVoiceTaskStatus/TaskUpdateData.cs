using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskUpdateData(
    List<TaskShortInfoDto> Tasks,
    string? ProjectName,
    string? Description,
    TaskItemStatus? Status,
    TaskItemPriority? Priority,
    DateTimeOffset? DueDate,
    Guid? ParentTaskId,
    TaskShortInfoDto? ParentTask,
    bool ConfirmationRequired = true
) : IVoiceTaskPayload;
