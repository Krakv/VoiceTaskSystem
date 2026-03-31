namespace TaskManager.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public enum VoiceIntent
{
    TASK_CREATE,
    TASK_UPDATE,
    TASK_DELETE,
    TASK_QUERY,
    UNKNOWN,
    AMBIGUOUS
}