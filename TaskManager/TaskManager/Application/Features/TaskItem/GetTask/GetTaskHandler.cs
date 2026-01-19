using MediatR;

namespace TaskManager.Application.Features.TaskItem.GetTask;

public sealed class GetTaskHandler : IRequestHandler<GetTaskQuery, GetTaskResponse>
{
    public Task<GetTaskResponse> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
