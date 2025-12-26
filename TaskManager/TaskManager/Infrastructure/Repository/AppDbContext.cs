using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Features.TaskItem;

namespace TaskManager.Infrastructure.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> TaskItems { get; set; }
}
