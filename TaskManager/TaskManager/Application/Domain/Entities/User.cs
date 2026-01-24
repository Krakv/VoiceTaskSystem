using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("User")]
public class User : IdentityUser<Guid>
{
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<CommandRequestItem> CommandRequests { get; set; } = [];

    public ICollection<TaskItem> Tasks { get; set; } = [];
}
