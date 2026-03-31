namespace TaskManager.Application.Features.CommandRequestItem.UpdateVoiceTask;

public sealed record UpdateVoiceTaskResponse(
    Dictionary<string, string> UpdatedFields
    );
