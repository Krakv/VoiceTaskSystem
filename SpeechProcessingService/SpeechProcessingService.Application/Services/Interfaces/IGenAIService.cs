namespace SpeechProcessingService.Application.Services.Services.Interfaces;

public interface IGenAIService
{
    Task<string> GetAnswer(string prompt);
}
