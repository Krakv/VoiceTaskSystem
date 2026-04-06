using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed record GetTasksQuery(
    string? Query,
    string? Status,
    string? Priority,
    string? SortBy,
    string? SordOrder,
    string? Limit = "20",
    string? Page = "0"
    ) : IRequest<GetTasksResponse>;
