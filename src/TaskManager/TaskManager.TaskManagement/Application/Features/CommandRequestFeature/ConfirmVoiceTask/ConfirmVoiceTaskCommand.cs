using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.ConfirmVoiceTask;

public sealed record ConfirmVoiceTaskCommand(Guid OwnerId, Guid CommandRequestId) : IRequest<ConfirmVoiceTaskResponse>;
