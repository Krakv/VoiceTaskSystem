namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed record Pagination(int Limit, int Page, int Total, int TotalPages);
