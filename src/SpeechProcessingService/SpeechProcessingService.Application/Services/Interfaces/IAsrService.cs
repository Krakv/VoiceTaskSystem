using SpeechProcessingService.Application.DTOs;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IAsrService
{
    Task<string> RecognizeSpeechAsync(AudioFile audioFile);
}
