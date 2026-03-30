namespace SpeechProcessingService.Application.Services.Services.Interfaces;

public interface IAsrService
{
    Task<string> RecognizeSpeechAsync(IFormFile audioFile);
}
