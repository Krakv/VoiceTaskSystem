namespace SpeechProcessingService.Application.Services.Services.Interfaces;

public interface IEntityExtractionService
{
    Task<Dictionary<string, string>> ExtractEntitiesAsync(string command, string intent);
}
