using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.DeleteVoiceTask;

public sealed record DeleteVoiceTaskCommand(string CommandRequestId) : IRequest<DeleteVoiceTaskResponse>;
