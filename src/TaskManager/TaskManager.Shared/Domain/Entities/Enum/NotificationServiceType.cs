using System.Text.Json.Serialization;

namespace TaskManager.Shared.Domain.Entities.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationServiceType
{
    Telegram = 1,
    Email = 2
}
