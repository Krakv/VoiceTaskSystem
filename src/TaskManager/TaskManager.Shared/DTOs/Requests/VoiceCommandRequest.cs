namespace TaskManager.Shared.DTOs.Requests;

public record VoiceCommandRequest(
    Guid CommandId,
    InputFile InputFile
);
