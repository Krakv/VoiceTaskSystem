using MediatR;
using TaskManager.Shared.Exceptions;
using TaskManager.Repository.Context;
using Microsoft.Extensions.Logging;
using TaskManager.Shared.Events;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.UpdateTask;

public sealed class UpdateTaskHandler(AppDbContext context, ILogger<UpdateTaskHandler> logger, IMediator mediator) : IRequestHandler<UpdateTaskCommand, Guid>
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<UpdateTaskHandler> _logger = logger;
    private readonly IMediator _mediator = mediator;

    public async Task<Guid> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([request.TaskId], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (request.OwnerId != task.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        task.ProjectName = request.ProjectName;
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;
        task.ParentTaskId = request.ParentTaskId;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new TaskUpdatedEvent(task.TaskId, request.OwnerId, $"Task {task.TaskId} fully updated"), cancellationToken);
        _logger.LogInformation("Updated task with id {TaskId}", task.TaskId);
        return task.TaskId;
    }
}
