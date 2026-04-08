using System.Text.Json.Serialization;

namespace TaskManager.RulesEngine.Domain.Actions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SetFieldAction), "SET_FIELD")]
[JsonDerivedType(typeof(CreateNotificationAction), "CREATE_NOTIFICATION")]
[JsonDerivedType(typeof(CreateCalendarEventAction), "CREATE_CALENDAR_EVENT")]
public abstract class RuleAction
{
    [JsonIgnore]
    public ActionType Type { get; set; }
}
