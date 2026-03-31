using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.DeleteVoiceTask;

public sealed record DeleteVoiceTaskCommand(string CommandRequestId) : IRequest<DeleteVoiceTaskResponse>;
