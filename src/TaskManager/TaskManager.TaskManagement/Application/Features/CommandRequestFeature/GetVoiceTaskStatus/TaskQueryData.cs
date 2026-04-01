namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskQueryData(List<TaskData> Tasks) : IVoiceTaskPayload;