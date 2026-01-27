using MediatR;

namespace TaskManager.Application.Features.TaskItem.GetTask;

public sealed record GetTaskQuery(string TaskId) : IRequest<GetTaskResponse>;
