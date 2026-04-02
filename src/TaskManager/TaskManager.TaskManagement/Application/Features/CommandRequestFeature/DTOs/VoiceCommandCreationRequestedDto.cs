using TaskManager.Shared.DTOs.Requests;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DTOs;

public class VoiceCommandCreationRequestedDto
{
    public required VoiceCommandRequest VoiceCommandRequest { get; init; }
    public Guid UserId { get; init; }
}
