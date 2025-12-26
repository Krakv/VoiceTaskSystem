namespace SpeechProcessingService.Application.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    Dictionary<string, string> Parameters
  );
