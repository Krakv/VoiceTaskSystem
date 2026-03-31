using SpeechProcessingService.Application.DTOs;

namespace SpeechProcessingService.Application.Features.Audio.DTOs;

public class AudioCommandRequest
{
    public AudioFile AudioFile { get; set; } = null!;
}
