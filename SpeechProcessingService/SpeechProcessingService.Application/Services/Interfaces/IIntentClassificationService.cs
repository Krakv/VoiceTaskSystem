namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IIntentClassificationService
{
    Task<string> ClassifyIntentAsync(string commandText);
    Task InitAsync();
}
