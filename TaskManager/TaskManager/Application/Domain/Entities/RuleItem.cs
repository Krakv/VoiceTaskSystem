using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("Rule")]
public class RuleItem
{
    [Key]
    public Guid RuleId { get; set; } = Guid.NewGuid();

    public required Guid OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public required User Owner { get; set; }

    public required string Event {  get; set; }
    public required string Condition { get; set; }
    public required string Action { get; set; }
    public required bool IsActive { get; set; }
}
