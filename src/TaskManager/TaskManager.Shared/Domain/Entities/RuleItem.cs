using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Shared.Domain.Entities.Enum;

namespace TaskManager.Shared.Domain.Entities;

[Table("Rule")]
public class RuleItem
{
    [Key]
    public Guid RuleId { get; set; } = Guid.NewGuid();

    public Guid OwnerId { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; } = null!;

    [Required]
    public RuleEvent Event { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Condition { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "jsonb")]
    public string Action { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}