namespace TaskManager.Shared.DTOs.Requests;

public record VoiceCommandRequest(
    Guid CommandId,
    InputFile FormFile,
    string UserTimeZone = "Europe/Moscow"
);
