namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public enum VoiceIntent
{
    TASK_CREATE,
    TASK_UPDATE,
    TASK_DELETE,
    TASK_QUERY,
    UNKNOWN,
    AMBIGUOUS
}