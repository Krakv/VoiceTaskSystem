using Microsoft.Extensions.Logging;
using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.DTOs.Responses;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Application.Services;

public class SpeechProcessingService(
    IIntentClassificationService intentClassifier, 
    IEntityExtractionService entityExtractor, 
    IAsrService asrService,
    IEntityNormalizer entityNormalizer,
    ILogger<SpeechProcessingService> logger) : ISpeechProcessingService
{
    private readonly IIntentClassificationService _intentClassifier = intentClassifier;
    private readonly IEntityExtractionService _entityExtractor = entityExtractor;
    private readonly IEntityNormalizer _entityNormalizer = entityNormalizer;
    private readonly IAsrService _asrService = asrService;
    private readonly ILogger<SpeechProcessingService> _logger = logger;

    public async Task<CommandResponse> ProcessCommandAsync(Guid commandRequestId, AudioFile audioFile, TimeZoneInfo userTimeZone)
    {
        var command = await _asrService.RecognizeSpeechAsync(audioFile);

        var intent = await _intentClassifier.ClassifyIntentAsync(command);
        _logger.LogDebug("Intent recognized as {Intent} | {CommandRequestId}", intent, commandRequestId);

        Dictionary<string, string> entities = new();

        if (intent != CommandIntent.Ambiguous || intent != CommandIntent.Unknown)
        {
            _logger.LogDebug("Started to extract entities | {CommandRequestId}", commandRequestId);
            entities = await _entityExtractor.ExtractEntitiesAsync(command);
            _logger.LogDebug("Entities extracted: {Entities} | {CommandRequestId}", entities, commandRequestId);
        }

        var entitiesNormalized = await _entityNormalizer.NormalizeAsync(entities, userTimeZone);

        var response = new CommandResponse(commandRequestId, command, intent, entitiesNormalized);
        _logger.LogDebug("Command Response was created: {CommandResponse} | {CommandRequestId}", response, commandRequestId);

        return response;
    }


}
