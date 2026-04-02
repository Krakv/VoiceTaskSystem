using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SpeechProcessingService.Application.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskItemStatus
{
    [EnumMember(Value = "new")]
    New = 0,

    [EnumMember(Value = "in_progress")]
    InProgress = 1,

    [EnumMember(Value = "done")]
    Done = 2,

    [EnumMember(Value = "canceled")]
    Canceled = 3
}
