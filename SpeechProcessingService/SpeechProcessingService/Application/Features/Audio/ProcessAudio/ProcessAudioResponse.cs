namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public record ProcessAudioResponse(
    Guid CommandId,
    Dictionary<string, string> Parameters
  );
