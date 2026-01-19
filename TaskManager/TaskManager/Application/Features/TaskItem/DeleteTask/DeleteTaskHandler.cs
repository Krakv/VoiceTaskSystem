using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Features.TaskItem.CreateTask;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Application.Features.TaskItem.DeleteTask;

public sealed class DeleteTaskHandler() : IRequestHandler<DeleteTaskCommand, DeleteTaskResponse>
{
    public Task<DeleteTaskResponse> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
