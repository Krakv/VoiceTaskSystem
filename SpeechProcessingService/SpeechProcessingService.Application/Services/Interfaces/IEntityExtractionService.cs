namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IEntityExtractionService
{
    Task<Dictionary<string, string>> ExtractEntitiesAsync(string command, string intent);
}
