using MediatR;

namespace TaskManager.Application.Features.TaskItem.DeleteTask;

public sealed record DeleteTaskCommand(string TaskId) : IRequest<string>;