namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IIntentClassificationService
{
    string ClassifyIntent(string commandText);
}
