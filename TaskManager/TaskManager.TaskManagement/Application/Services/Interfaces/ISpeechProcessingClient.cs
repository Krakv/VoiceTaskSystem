using TaskManager.Application.Common.DTOs.Responses;
using TaskManager.Application.Common.DTOs.Requests;

namespace TaskManager.TaskManagement.Application.Services.Interfaces;

public interface ISpeechProcessingClient
{
    Task<CommandResponse?> SendCommand(CommandRequest command);
}
