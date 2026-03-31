using MediatR;
using TaskManager.Shared.Exceptions;
using TaskManager.Repository.Context;
using TaskManager.TaskManagement.Interfaces;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskItem.GetTask;

public sealed class GetTaskHandler(AppDbContext context, ICurrentUser user, ILogger<GetTaskHandler> logger) : IRequestHandler<GetTaskQuery, GetTaskResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<GetTaskHandler> _logger = logger;

    public async Task<GetTaskResponse> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([Guid.Parse(request.TaskId)], cancellationToken: cancellationToken) 
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (_user.UserId != task.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        _logger.LogDebug("Task with id {TaskId} has been requested", task.TaskId);
        return new GetTaskResponse(task.Title, task.ProjectName, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt, task.UpdatedAt, task.ParentTaskId?.ToString());
    }
}