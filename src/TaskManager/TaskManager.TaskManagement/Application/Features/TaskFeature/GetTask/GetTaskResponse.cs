using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

public sealed record GetTaskResponse(
    Guid TaskId,
    string Title,
    string? ProjectName,
    string? Description,
    TaskItemStatus Status,
    TaskItemPriority Priority,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    TaskShortInfoDto? ParentTask,
    List<TaskShortInfoDto>? ChildrenTasks
    );
