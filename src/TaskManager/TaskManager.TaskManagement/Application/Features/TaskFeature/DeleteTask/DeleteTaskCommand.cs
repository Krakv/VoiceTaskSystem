using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.DeleteTask;

public sealed record DeleteTaskCommand(string TaskId) : IRequest<string>;