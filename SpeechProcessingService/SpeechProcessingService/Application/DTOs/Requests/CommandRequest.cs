namespace SpeechProcessingService.Application.DTOs.Requests;

public record CommandRequest(
    Guid CommandId,
    string CommandText
);
