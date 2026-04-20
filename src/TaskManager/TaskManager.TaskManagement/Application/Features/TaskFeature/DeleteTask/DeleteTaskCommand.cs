using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;

public sealed record DeleteTaskCommand(Guid OwnerId, Guid TaskId) : IRequest<Guid>;