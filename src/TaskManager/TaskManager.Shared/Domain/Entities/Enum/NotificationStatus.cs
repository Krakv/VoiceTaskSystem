using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationStatus
{
    Pending = 0,
    Processing = 1,
    Sent = 2,
    Failed = 3,
    Cancelled = 4,
}
