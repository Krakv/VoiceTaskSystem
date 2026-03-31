using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Services;

public class SpeechProcessingService(
    IIntentClassificationService intentClassifier, 
    IEntityExtractionService entityExtractor, 
    IAsrService asrService) : ISpeechProcessingService
{
    private readonly IIntentClassificationService _intentClassifier = intentClassifier;
    private readonly IEntityExtractionService _entityExtractor = entityExtractor;
    private readonly IAsrService _asrService = asrService;

    public async Task<CommandResponse> ProcessCommandAsync(AudioFile audioFile)
    {
        var command = await _asrService.RecognizeSpeechAsync(audioFile);

        var intent = await _intentClassifier.ClassifyIntentAsync(command);

        Dictionary<string, string> parameters = await _entityExtractor.ExtractEntitiesAsync(command);

        parameters.Add("intent", intent);

        var response = new CommandResponse(Guid.NewGuid(), command, parameters);

        return response;
    }
}
