namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.UpdateVoiceTask;

public sealed record UpdateVoiceTaskResponse(
    Dictionary<string, string> UpdatedFields
    );
