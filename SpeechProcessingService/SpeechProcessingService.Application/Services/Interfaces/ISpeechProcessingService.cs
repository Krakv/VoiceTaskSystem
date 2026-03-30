using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface ISpeechProcessingService
{
    Task<CommandResponse> ProcessCommandAsync(IFormFile audioFile);
}
