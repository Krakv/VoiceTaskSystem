using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TaskManager.Application.Domain.Entities;

[Table("CommandRequest")]
public class CommandRequestItem
{
    [Key]
    public required Guid CommandRequestId { get; set; }
    public required Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public required User Owner { get; set; }

    public required string Intent {  get; set; }
    public required JsonDocument Payload { get; set; }
    public required string Status { get; set; }
    public required bool ConfirmationRequired { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get;set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }


    public ICollection<TaskItem> TaskItems { get; set; } = [];
}
