using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

public sealed record GetTaskQuery(Guid OwnerId, Guid TaskId) : IRequest<GetTaskResponse>;
