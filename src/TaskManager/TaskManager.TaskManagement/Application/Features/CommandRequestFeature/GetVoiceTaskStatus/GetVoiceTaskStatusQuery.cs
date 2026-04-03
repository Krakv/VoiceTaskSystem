using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusQuery(string commandRequestId) : IRequest<GetVoiceTaskStatusResponse>;
