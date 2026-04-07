using TaskManager.Shared.DTOs.Responses;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public record TaskDeleteData(
    List<TaskShortInfoDto> Tasks,
    bool ConfirmationRequired = false
    ) : IVoiceTaskPayload;
