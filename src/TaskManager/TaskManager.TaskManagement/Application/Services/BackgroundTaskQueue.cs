using System.Threading.Channels;
using TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DTOs;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.TaskManagement.Application.Services;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<VoiceCommandCreationRequestedDto> _queue = Channel.CreateUnbounded<VoiceCommandCreationRequestedDto>();

    public async Task EnqueueAsync(VoiceCommandCreationRequestedDto job)
    {
        await _queue.Writer.WriteAsync(job);
    }

    public async Task<VoiceCommandCreationRequestedDto> DequeueAsync(CancellationToken ct)
    {
        return await _queue.Reader.ReadAsync(ct);
    }
}
