using MediatR;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Application.Features.TaskItem.CreateTask;

public sealed class CreateTaskHandler() : IRequestHandler<CreateTaskCommand, CreateTaskResponse>
{
    public Task<CreateTaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
