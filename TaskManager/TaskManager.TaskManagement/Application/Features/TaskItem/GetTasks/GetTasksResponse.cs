namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed record GetTasksResponse(List<TaskListElement> Tasks, Pagination Pagination);
