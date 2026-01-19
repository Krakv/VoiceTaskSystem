using MediatR;

namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed record GetTasksQuery(
    string OwnerId,
    string? Status,
    string? Priority,
    string? SortBy,
    string? SordOrder,
    string? limit = "20",
    string? offset = "0"
    ) : IRequest<GetTasksResponse>;
