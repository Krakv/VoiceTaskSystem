namespace TaskManager.Application.Features.TaskItem.CreateTask;

public record CreateTaskCommand(Dictionary<string, string> taskParameters, long chatId);
