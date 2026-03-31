namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IEntityExtractionService
{
    Task<Dictionary<string, string>> ExtractEntitiesAsync(string commandText);
    Task InitAsync();
}
