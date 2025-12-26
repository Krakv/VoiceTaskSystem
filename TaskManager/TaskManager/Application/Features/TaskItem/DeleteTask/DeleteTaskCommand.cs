namespace TaskManager.Application.Features.TaskItem.DeleteTask;

public record DeleteTaskCommand(string taskName, long chatId);
