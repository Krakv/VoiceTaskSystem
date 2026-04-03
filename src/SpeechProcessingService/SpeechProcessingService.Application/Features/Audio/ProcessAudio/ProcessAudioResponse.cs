using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Models;

namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public record ProcessAudioResponse(
    Guid CommandId,
    string TextRecognized,
    CommandIntent CommandIntent,
    TaskItem Entities
  );
