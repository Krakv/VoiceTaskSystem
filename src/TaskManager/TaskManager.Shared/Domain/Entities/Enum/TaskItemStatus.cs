using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemStatus
{
    New = 0,
    InProgress = 1,
    Done = 2,
    Canceled = 3
}
