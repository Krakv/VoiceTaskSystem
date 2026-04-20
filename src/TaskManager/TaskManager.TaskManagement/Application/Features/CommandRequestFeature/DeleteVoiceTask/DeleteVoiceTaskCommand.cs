using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DeleteVoiceTask;

public sealed record DeleteVoiceTaskCommand(Guid OwnerId, Guid CommandRequestId) : IRequest<DeleteVoiceTaskResponse>;
