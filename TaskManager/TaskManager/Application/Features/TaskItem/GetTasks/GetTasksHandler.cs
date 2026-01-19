using MediatR;

namespace TaskManager.Application.Features.TaskItem.GetTasks;

public sealed class GetTasksHandler : IRequestHandler<GetTasksQuery, GetTasksResponse>
{
    public Task<GetTasksResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
