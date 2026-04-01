using TaskManager.Shared.DTOs.Requests;
using TaskManager.Shared.Events;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.Events.VoiceCommandCreationRequested;

public class VoiceCommandCreationRequestedEvent : BaseEvent
{
    public required VoiceCommandRequest VoiceCommandRequest { get; set; }
    public override Guid UserId { get; init; }
    public override string Event { get; init; } = nameof(VoiceCommandCreationRequestedEvent);
}
