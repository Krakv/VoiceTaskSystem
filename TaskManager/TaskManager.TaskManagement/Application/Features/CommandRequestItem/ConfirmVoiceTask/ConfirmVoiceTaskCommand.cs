using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.ConfirmVoiceTask;

public sealed record ConfirmVoiceTaskCommand(string CommandRequestId) : IRequest<ConfirmVoiceTaskResponse>;
