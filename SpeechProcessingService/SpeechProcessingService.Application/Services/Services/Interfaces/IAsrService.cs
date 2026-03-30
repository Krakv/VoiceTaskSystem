namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IAsrService
{
    Task<string> RecognizeSpeechAsync(IFormFile audioFile);
}
