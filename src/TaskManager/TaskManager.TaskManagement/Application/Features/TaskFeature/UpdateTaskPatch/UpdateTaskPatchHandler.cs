using MediatR;
using System.Globalization;
using Microsoft.Extensions.Logging;
using TaskManager.Repository.Context;
using TaskManager.Shared.Domain.Entities.Enum;
using TaskManager.Shared.Events;
using TaskManager.Shared.Exceptions;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTaskPatch;

public sealed class UpdateTaskPatchHandler(AppDbContext context, ILogger<UpdateTaskPatchHandler> logger, IMediator mediator) : IRequestHandler<UpdateTaskPatchCommand, Guid>
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<UpdateTaskPatchHandler> _logger = logger; 
    private readonly IMediator _mediator = mediator;

    public async Task<Guid> Handle(UpdateTaskPatchCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([request.TaskId], cancellationToken)
        ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (request.OwnerId != task.OwnerId)
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");

        if (request.ProjectName is not null) task.ProjectName = request.ProjectName;
        if (request.Title is not null) task.Title = request.Title;
        if (request.Description is not null) task.Description = request.Description;
        if (request.DueDate is not null) task.DueDate = string.IsNullOrEmpty(request.DueDate) ? null : DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture);
        if (request.Status is not null) task.Status = (TaskItemStatus)request.Status;
        if (request.Priority is not null) task.Priority = (TaskItemPriority)request.Priority;
        if (request.ParentTaskId is not null)
            task.ParentTaskId = string.IsNullOrEmpty(request.ParentTaskId) ? null : Guid.Parse(request.ParentTaskId);

        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new TaskUpdatedEvent(task.TaskId, request.OwnerId, $"Task {task.TaskId} partially updated"), cancellationToken);
        _logger.LogInformation("Updated task with id {TaskId}", task.TaskId);
        return task.TaskId;
    }
}
