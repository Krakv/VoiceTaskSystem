using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskManager.Application.Domain.Entities;

namespace TaskManager.Infrastructure.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> TaskItems { get; set; }
}
