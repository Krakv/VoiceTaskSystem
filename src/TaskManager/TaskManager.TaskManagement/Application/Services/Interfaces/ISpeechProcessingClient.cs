using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.DTOs.Requests;

namespace TaskManager.TaskManagement.Application.Services.Interfaces;

public interface ISpeechProcessingClient
{
    Task<CommandResponse?> SendCommand(VoiceCommandRequest command);
}
