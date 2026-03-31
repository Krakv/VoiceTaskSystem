namespace TaskManager.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public record TaskQueryData(List<TaskData> Tasks) : IVoiceTaskPayload;