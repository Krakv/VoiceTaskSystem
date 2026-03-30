namespace SpeechProcessingService.Application.Features.Audio.DTOs;

public record CommandRequest(
    Guid CommandId,
    string CommandText
);
