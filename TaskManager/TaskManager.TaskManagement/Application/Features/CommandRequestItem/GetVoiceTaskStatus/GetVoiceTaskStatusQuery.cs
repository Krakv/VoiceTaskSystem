using MediatR;

namespace TaskManager.Application.Features.CommandRequestItem.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusQuery(string commandRequestId) : IRequest<GetVoiceTaskStatusResponse>;
