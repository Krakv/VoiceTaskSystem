using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Shared.Domain.Entities;

[Table("User")]
public class User : IdentityUser<Guid>
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = [];
    public ICollection<CommandRequestItem> CommandRequests { get; set; } = [];
    public ICollection<ExternalCalendarAccount> ExternalCalendarAccounts { get; set; } = [];
}