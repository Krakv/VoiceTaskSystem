using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommandRequestStatus
{
    [EnumMember(Value = "pending")]
    Pending = 0,

    [EnumMember(Value = "processing")]
    Processing = 1,

    [EnumMember(Value = "accepted")]
    Accepted = 2,

    [EnumMember(Value = "cancelled")]
    Cancelled = 3,

    [EnumMember(Value = "failed")]
    Failed = 4,
}
