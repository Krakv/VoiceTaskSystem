using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Services;

public class SpeechProcessingService(IIntentClassificationService intentClassifier, IEntityExtractionService entityExtractor) : ISpeechProcessingService
{
    private readonly IIntentClassificationService _intentClassifier = intentClassifier;
    private readonly IEntityExtractionService _entityExtractor = entityExtractor;

    public CommandResponse ProcessCommand(string command)
    {
        var intent = _intentClassifier.ClassifyIntent(command);

        Dictionary<string, string> parameters = new();

        if (intent == "create_task" || intent == "delete_task")
        {
            parameters = _entityExtractor.ExtractEntities(command, intent);
        }

        parameters.Add("intent", intent);

        var response = new CommandResponse(Guid.NewGuid(), parameters);

        return response;
    }
}
