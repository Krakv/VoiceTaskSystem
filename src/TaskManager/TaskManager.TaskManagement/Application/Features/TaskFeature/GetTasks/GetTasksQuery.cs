using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed record GetTasksQuery(
    Guid OwnerId,
    string? Query,
    string? Status,
    string? Priority,
    string? SortBy,
    string? SortOrder,
    int? Limit = 20,
    int? Page = 0
    ) : IRequest<GetTasksResponse>;
