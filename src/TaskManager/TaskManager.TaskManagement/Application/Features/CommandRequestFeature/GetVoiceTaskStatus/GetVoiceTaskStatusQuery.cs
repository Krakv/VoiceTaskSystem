using MediatR;

namespace TaskManager.TaskManagement.Application.Features.CommandRequestFeature.GetVoiceTaskStatus;

public sealed record GetVoiceTaskStatusQuery(Guid OwnerId, Guid CommandRequestId) : IRequest<GetVoiceTaskStatusResponse>;
