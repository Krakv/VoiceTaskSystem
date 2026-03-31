using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaskManager.Application.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemPriority
{
    [EnumMember(Value = "low")]
    Low = 1,

    [EnumMember(Value = "medium")]
    Medium = 2,

    [EnumMember(Value = "high")]
    High = 3
}
