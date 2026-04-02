using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DTOs;

namespace TaskManager.TaskManagement.Application.Services.Interfaces;

public interface IBackgroundTaskQueue
{
    Task EnqueueAsync(VoiceCommandCreationRequestedDto job);
    Task<VoiceCommandCreationRequestedDto> DequeueAsync(CancellationToken ct);
}
