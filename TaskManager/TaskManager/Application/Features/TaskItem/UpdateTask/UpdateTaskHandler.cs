using MediatR;

namespace TaskManager.Application.Features.TaskItem.UpdateTask;

public sealed class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResponse>
{
    public Task<UpdateTaskResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
