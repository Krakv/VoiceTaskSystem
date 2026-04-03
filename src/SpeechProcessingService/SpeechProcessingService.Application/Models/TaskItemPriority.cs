using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SpeechProcessingService.Application.Models;

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
