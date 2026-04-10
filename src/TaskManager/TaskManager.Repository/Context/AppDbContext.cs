using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Repository.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<CalendarEvent> CalendarEvent { get; set; }
    public DbSet<CommandRequestItem> CommandRequestItem { get; set; }
    public DbSet<ExternalCalendarAccount> ExternalCalendarAccount { get; set; }
    public DbSet<NotificationItem> NotificationItem { get; set; }
    public DbSet<RuleItem> RuleItem { get; set; }
    public DbSet<ServiceItem> ServiceItem { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ServiceItem>()
            .HasIndex(x => x.ServiceName)
            .IsUnique();

        builder.Entity<ExternalCalendarAccount>()
            .HasIndex(x => new { x.OwnerId, x.BaseUrl })
            .IsUnique();

        builder.Entity<ServiceItem>().HasData(
            new ServiceItem { ServiceId = 1, ServiceName = "Telegram" },
            new ServiceItem { ServiceId = 2, ServiceName = "Email" },
            new ServiceItem { ServiceId = 3, ServiceName = "Push" }
        );
    }
}
