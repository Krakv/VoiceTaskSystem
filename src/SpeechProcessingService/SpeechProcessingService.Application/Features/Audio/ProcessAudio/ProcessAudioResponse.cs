namespace SpeechProcessingService.Application.Features.Audio.ProcessAudio;

public record ProcessAudioResponse(
    Guid CommandId,
    string TextRecognized,
    Dictionary<string, string> Parameters
  );
