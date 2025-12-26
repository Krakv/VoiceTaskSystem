
namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IEntityExtractionService
{
    Dictionary<string, string> ExtractEntities(string command, string intent);
}
