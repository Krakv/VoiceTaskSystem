using SpeechProcessingService.Application.Models;

namespace SpeechProcessingService.Application.DTOs.Responses;

public record CommandResponse(
    Guid CommandId,
    string TextRecognized,
    CommandIntent CommandIntent,
    TaskItem Entities
  );
