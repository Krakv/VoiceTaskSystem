namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record MessageData(string Message) : IVoiceTaskPayload;
