namespace SpeechProcessingService.Application.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    string TextRecognized,
    Dictionary<string, string> Parameters
  );
