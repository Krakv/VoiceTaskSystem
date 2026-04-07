
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.DTOs.Responses;

public record TaskShortInfoDto(Guid TaskId, string Title, TaskItemStatus status, TaskItemPriority Priority, DateTimeOffset? dueDate);
