using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface ISpeechProcessingService
{
    Task<CommandResponse> ProcessCommandAsync(AudioFile audioFile);
}
