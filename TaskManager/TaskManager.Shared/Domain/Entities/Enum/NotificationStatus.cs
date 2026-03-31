using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationStatus
{
    [EnumMember(Value = "pending")]
    Pending = 0,

    [EnumMember(Value = "sent")]
    Sent = 1,

    [EnumMember(Value = "failed")]
    Failed = 2,

    [EnumMember(Value = "cancelled")]
    Cancelled = 3,
}
