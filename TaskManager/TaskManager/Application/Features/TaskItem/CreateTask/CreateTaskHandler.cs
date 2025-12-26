using TaskManager.Infrastructure.Repository;

namespace TaskManager.Application.Features.TaskItem.CreateTask;

public class CreateTaskHandler(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<TaskItem> CreateTask(CreateTaskCommand command)
    {
        var dueDate = command.taskParameters.GetValueOrDefault("due_date", "");

        var task = new TaskItem()
        {
            OwnerId = command.chatId,
            Title = command.taskParameters.GetValueOrDefault("name", ""),
            Description = command.taskParameters.GetValueOrDefault("description", ""),
            ProjectName = command.taskParameters.GetValueOrDefault("project_name", ""),
            Priority = command.taskParameters.GetValueOrDefault("priority", ""),
            DueDate = dueDate == "" ? null : DateTimeOffset.Parse(dueDate),
            Status = "Ожидает выполнения"
        };

        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();

        return task;
    }
}
