using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Services;

public class SpeechProcessingService(IIntentClassificationService intentClassifier, IEntityExtractionService entityExtractor) : ISpeechProcessingService
{
    private readonly IIntentClassificationService _intentClassifier = intentClassifier;
    private readonly IEntityExtractionService _entityExtractor = entityExtractor;

    public async Task<CommandResponse> ProcessCommandAsync(string command)
    {
        var intent = await _intentClassifier.ClassifyIntentAsync(command);

        Dictionary<string, string> parameters = new();

        parameters.Add("intent", intent);

        var response = new CommandResponse(Guid.NewGuid(), parameters);

        return response;
    }
}
