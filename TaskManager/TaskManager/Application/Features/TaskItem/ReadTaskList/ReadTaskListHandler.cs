using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Application.Features.TaskItem.ReadTaskList;

public class ReadTaskListHandler(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<TaskItem>> ReadTaskList(ReadTaskListCommand command)
    {
        var tasks = await _context.TaskItems
            .Where(x => x.OwnerId == command.ownerId)
            .OrderByDescending(x => x.Priority == "high" ? 3 : x.Priority == "medium" ? 2 : x.Priority == "low" ? 1 : 0)
            .ThenBy(x => x.DueDate)
            .ToListAsync();

        return tasks;
    }
}
