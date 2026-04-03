using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IIntentClassificationService
{
    Task<CommandIntent> ClassifyIntentAsync(string commandText);
    Task InitAsync();
}
