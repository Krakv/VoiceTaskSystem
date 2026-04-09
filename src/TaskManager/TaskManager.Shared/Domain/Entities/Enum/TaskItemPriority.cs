using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}
