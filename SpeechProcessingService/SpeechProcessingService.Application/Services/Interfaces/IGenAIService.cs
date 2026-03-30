namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IGenAIService
{
    Task<string> GetAnswer(string prompt);
}
