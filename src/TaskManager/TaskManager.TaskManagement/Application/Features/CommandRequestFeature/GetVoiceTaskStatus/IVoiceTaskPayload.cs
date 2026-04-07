using System.Text.Json.Serialization;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

[JsonPolymorphic]
[JsonDerivedType(typeof(TaskCreateData))]
[JsonDerivedType(typeof(TaskDeleteData))]
[JsonDerivedType(typeof(TaskUpdateData))]
[JsonDerivedType(typeof(TaskQueryData))]
[JsonDerivedType(typeof(MessageData))]
public interface IVoiceTaskPayload { }
