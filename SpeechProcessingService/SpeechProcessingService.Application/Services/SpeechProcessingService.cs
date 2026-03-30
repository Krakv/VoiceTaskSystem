using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Services.Interfaces;

namespace SpeechProcessingService.Application.Services.Services;

public class SpeechProcessingService(
    IIntentClassificationService intentClassifier, 
    IEntityExtractionService entityExtractor, 
    IAsrService asrService) : ISpeechProcessingService
{
    private readonly IIntentClassificationService _intentClassifier = intentClassifier;
    private readonly IEntityExtractionService _entityExtractor = entityExtractor;
    private readonly IAsrService _asrService = asrService;

    public async Task<CommandResponse> ProcessCommandAsync(IFormFile audioFile)
    {
        var command = await _asrService.RecognizeSpeechAsync(audioFile);

        var intent = await _intentClassifier.ClassifyIntentAsync(command);

        Dictionary<string, string> parameters = new();

        if (intent == "create_task" || intent == "delete_task")
        {
            parameters = await _entityExtractor.ExtractEntitiesAsync(command, intent);
        }

        parameters.Add("intent", intent);

        var response = new CommandResponse(Guid.NewGuid(), parameters);

        return response;
    }
}
