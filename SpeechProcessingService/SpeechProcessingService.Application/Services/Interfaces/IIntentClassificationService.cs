namespace SpeechProcessingService.Application.Services.Services.Interfaces;

public interface IIntentClassificationService
{
    Task<string> ClassifyIntentAsync(string commandText);
}
