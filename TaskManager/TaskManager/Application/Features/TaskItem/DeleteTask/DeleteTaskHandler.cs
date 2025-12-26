using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Features.TaskItem.CreateTask;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Application.Features.TaskItem.DeleteTask;

public class DeleteTaskHandler(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<bool> DeleteTask(DeleteTaskCommand command)
    {
        var task = await _context.TaskItems
        .FirstOrDefaultAsync(t => t.OwnerId == command.chatId && t.Title == command.taskName);

        if (task == null)
            return false;

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}
