using Microsoft.Extensions.Logging;
using TaskManager.Shared.EventHandlers;
using TaskManager.TaskManagement.Application.Services.Interfaces;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.Events.VoiceCommandCreationRequested;

public class VoiceCommandCreationRequestedEventHandler(
    ILogger<VoiceCommandCreationRequestedEventHandler> logger,
    ISpeechProcessingClient client)
    : BaseEventHandler<VoiceCommandCreationRequestedEvent>(logger)
{
    private readonly ISpeechProcessingClient _client = client;

    protected override async Task Process(VoiceCommandCreationRequestedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        var interpretedText = await _client.SendCommand(notification.VoiceCommandRequest);
    }
}
