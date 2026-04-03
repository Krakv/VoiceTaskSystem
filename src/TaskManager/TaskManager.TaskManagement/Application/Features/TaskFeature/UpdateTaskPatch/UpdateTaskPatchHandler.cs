using MediatR;
using System.Globalization;
using TaskManager.Shared.Exceptions;
using TaskManager.Repository.Context;
using TaskManager.TaskManagement.Interfaces;
using TaskManager.Shared.Domain.Entities.Enum;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

public sealed class UpdateTaskPatchHandler(AppDbContext context, ICurrentUser user, ILogger<UpdateTaskPatchHandler> logger) : IRequestHandler<UpdateTaskPatchCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<UpdateTaskPatchHandler> _logger = logger; 

    public async Task<string> Handle(UpdateTaskPatchCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([Guid.Parse(request.TaskId)], cancellationToken)
        ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (_user.UserId != task.OwnerId)
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");

        if (request.ProjectName is not null) task.ProjectName = request.ProjectName;
        if (request.Title is not null) task.Title = request.Title;
        if (request.Description is not null) task.Description = request.Description;
        if (request.Status is not null) task.Status = Enum.Parse<TaskItemStatus>(request.Status, ignoreCase: true);
        if (request.Priority is not null) task.Priority = Enum.Parse<TaskItemPriority>(request.Priority, ignoreCase: true);
        if (request.DueDate is not null) task.DueDate = string.IsNullOrEmpty(request.DueDate) ? null : DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture);
        if (request.ParentTaskId is not null)
            task.ParentTaskId = string.IsNullOrEmpty(request.ParentTaskId) ? null : Guid.Parse(request.ParentTaskId);

        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated task with id {TaskId}", task.TaskId);
        return task.TaskId.ToString();
    }
}
