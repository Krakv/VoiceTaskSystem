using MediatR;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed record GetTasksQuery(
    Guid OwnerId,
    string? Query,
    TaskItemStatus? Status,
    TaskItemPriority? Priority,
    string? SortBy,
    string? SortOrder,
    int? Limit = 20,
    int? Page = 0
    ) : IRequest<GetTasksResponse>;
