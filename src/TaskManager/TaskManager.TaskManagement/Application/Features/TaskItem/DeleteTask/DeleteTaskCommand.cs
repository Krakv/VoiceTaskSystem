using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskItem.DeleteTask;

public sealed record DeleteTaskCommand(string TaskId) : IRequest<string>;