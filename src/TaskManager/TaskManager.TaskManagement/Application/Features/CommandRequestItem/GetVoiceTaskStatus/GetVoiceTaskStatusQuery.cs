using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusQuery(string commandRequestId) : IRequest<GetVoiceTaskStatusResponse>;
