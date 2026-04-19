using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.DTOs.Responses;
using TaskManager.Shared.Exceptions;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTask;

public sealed class GetTaskHandler(AppDbContext context, ILogger<GetTaskHandler> logger) : IRequestHandler<GetTaskQuery, GetTaskResponse>
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<GetTaskHandler> _logger = logger;

    public async Task<GetTaskResponse> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems
            .Include(t => t.Parent)
            .Include(t => t.Children)
            .FirstOrDefaultAsync(t => t.TaskId == request.TaskId && t.OwnerId == request.OwnerId, cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        TaskShortInfoDto? parent = null;

        if (task.ParentTaskId != null && task.Parent != null)
        {
            parent = new(task.Parent.TaskId, task.Parent.Title, task.Parent.Status, task.Parent.Priority, task.Parent.DueDate);
        }

        List<TaskShortInfoDto> children = [.. task.Children.Select(x => new TaskShortInfoDto(x.TaskId, x.Title, x.Status, x.Priority, x.DueDate))];

        _logger.LogDebug("Task with id {TaskId} has been requested", task.TaskId);
        return new GetTaskResponse(task.TaskId, task.Title, task.ProjectName, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt, task.UpdatedAt, parent, children);
    }
}