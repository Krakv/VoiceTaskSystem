using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaskManager.TaskManagement.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommandIntent
{
    [EnumMember(Value = "TASK_CREATE")]
    TaskCreate = 1,

    [EnumMember(Value = "TASK_UPDATE")]
    TaskUpdate = 2,

    [EnumMember(Value = "TASK_DELETE")]
    TaskDelete = 3,

    [EnumMember(Value = "TASK_QUERY")]
    TaskQuery = 4,

    [EnumMember(Value = "UNKNOWN")]
    Unknown = 5,

    [EnumMember(Value = "AMBIGUOUS")]
    Ambiguous = 6,
}
