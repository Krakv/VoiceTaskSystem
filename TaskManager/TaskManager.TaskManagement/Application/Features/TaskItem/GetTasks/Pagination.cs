namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed record Pagination(int Limit, int Page, int Total, int TotalPages);
