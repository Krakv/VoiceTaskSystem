using MediatR;
using System.Globalization;
using TaskManager.Shared.Exceptions;
using TaskManager.Repository.Context;
using TaskManager.TaskManagement.Interfaces;
using TaskManager.Shared.Domain.Entities.Enum;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskItem.UpdateTask;

public sealed class UpdateTaskHandler(AppDbContext context, ICurrentUser user, ILogger<UpdateTaskHandler> logger) : IRequestHandler<UpdateTaskCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<UpdateTaskHandler> _logger = logger;

    public async Task<string> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([Guid.Parse(request.TaskId)], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (_user.UserId != task.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        task.ProjectName = request.ProjectName;
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = Enum.Parse<TaskItemStatus>(request.Status, ignoreCase: true);
        task.Priority = Enum.Parse<TaskItemPriority>(request.Priority, ignoreCase: true);
        task.DueDate = string.IsNullOrEmpty(request.DueDate) ? null : DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture);
        task.ParentTaskId = string.IsNullOrEmpty(request.ParentTaskId) ? null : Guid.Parse(request.ParentTaskId);
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated task with id {TaskId}", task.TaskId);
        return task.TaskId.ToString();
    }
}
