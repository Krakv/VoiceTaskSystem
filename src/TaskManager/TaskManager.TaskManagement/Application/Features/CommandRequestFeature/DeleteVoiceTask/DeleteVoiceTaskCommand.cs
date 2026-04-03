using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.DeleteVoiceTask;

public sealed record DeleteVoiceTaskCommand(string CommandRequestId) : IRequest<DeleteVoiceTaskResponse>;
