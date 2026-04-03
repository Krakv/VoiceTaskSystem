namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public sealed record GetTasksResponse(List<TaskListElement> Tasks, Pagination Pagination);
