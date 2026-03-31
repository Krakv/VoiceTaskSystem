using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskItem.GetTasks;

public sealed record GetTasksQuery(
    string? Status,
    string? Priority,
    string? SortBy,
    string? SordOrder,
    string? Limit = "20",
    string? Page = "0"
    ) : IRequest<GetTasksResponse>;
