using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.Domain.Entities;

[Table("CommandRequest")]
public class CommandRequestItem
{
    [Key]
    public Guid CommandRequestId { get; set; } = Guid.NewGuid();

    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    public CommandIntent? Intent { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Payload { get; set; }

    [Required]
    public CommandRequestStatus Status { get; set; } = CommandRequestStatus.Pending;

    public bool ConfirmationRequired { get; set; } = false;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }

    public ICollection<TaskItem> TaskItems { get; set; } = [];
}