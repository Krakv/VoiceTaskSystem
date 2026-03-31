using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.ConfirmVoiceTask;

public sealed record ConfirmVoiceTaskCommand(string CommandRequestId) : IRequest<ConfirmVoiceTaskResponse>;
