using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Exceptions;
using TaskManager.TaskManagement.Interfaces;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

public sealed class GetTaskHandler(AppDbContext context, ICurrentUser user, ILogger<GetTaskHandler> logger) : IRequestHandler<GetTaskQuery, GetTaskResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<GetTaskHandler> _logger = logger;

    public async Task<GetTaskResponse> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(t => t.Parent)
            .Include(t => t.Children)
            .FirstOrDefaultAsync(t => t.TaskId == Guid.Parse(request.TaskId), cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (_user.UserId != task.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        TaskShortInfoDto? parent = null;

        if (task.ParentTaskId != null && task.Parent != null)
        {
            parent = new(task.Parent.TaskId, task.Parent.Title, task.Parent.Status, task.Parent.Priority, task.Parent.DueDate);
        }

        List<TaskShortInfoDto> children = task.Children
            .Select(x => new TaskShortInfoDto(x.TaskId, x.Title, x.Status, x.Priority, x.DueDate))
            .ToList();

        _logger.LogDebug("Task with id {TaskId} has been requested", task.TaskId);
        return new GetTaskResponse(task.Title, task.ProjectName, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt, task.UpdatedAt, parent, children);
    }
}