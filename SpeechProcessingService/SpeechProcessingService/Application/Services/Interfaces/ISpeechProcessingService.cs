using SpeechProcessingService.Application.DTOs.Responses;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface ISpeechProcessingService
{
    CommandResponse ProcessCommand(string command);
}
