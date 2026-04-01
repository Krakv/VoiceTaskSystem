using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.ConfirmVoiceTask;

public sealed record ConfirmVoiceTaskCommand(string CommandRequestId) : IRequest<ConfirmVoiceTaskResponse>;
