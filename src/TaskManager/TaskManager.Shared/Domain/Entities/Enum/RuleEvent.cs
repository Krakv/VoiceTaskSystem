using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RuleEvent
{
    [EnumMember(Value = "task.created")]
    TaskCreated = 1,

    [EnumMember(Value = "task.updated")]
    TaskUpdated = 2,

    [EnumMember(Value = "task.deleted")]
    TaskDeleted = 3,

    [EnumMember(Value = "task.completed")]
    TaskCompleted = 4,

    [EnumMember(Value = "task.overdue")]
    TaskOverdue = 5,
}