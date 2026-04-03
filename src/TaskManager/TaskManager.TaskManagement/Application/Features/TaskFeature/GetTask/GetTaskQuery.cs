using MediatR;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

public sealed record GetTaskQuery(string TaskId) : IRequest<GetTaskResponse>;
