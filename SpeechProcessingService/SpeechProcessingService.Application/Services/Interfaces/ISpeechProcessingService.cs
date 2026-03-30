using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Services.Services.Interfaces;

public interface ISpeechProcessingService
{
    Task<CommandResponse> ProcessCommandAsync(IFormFile audioFile);
}
