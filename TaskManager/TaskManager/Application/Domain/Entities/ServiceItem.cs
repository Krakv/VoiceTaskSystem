using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Application.Domain.Entities;

[Table("Service")]
public class ServiceItem
{
    [Key]
    public int ServiceId { get; set; }
    public required string ServiceName { get; set; } 
}
