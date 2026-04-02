using SpeechProcessingService.Application.DTOs;
using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface ISpeechProcessingService
{
    Task<CommandResponse> ProcessCommandAsync(Guid commandRequestId, AudioFile audioFile, TimeZoneInfo userTimeZone);
}
