using MediatR;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Repository.Context;
using TaskManager.Shared.Events;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

public sealed class CreateTaskHandler(AppDbContext context, ILogger<CreateTaskHandler> logger, IMediator mediator) : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<CreateTaskHandler> _logger = logger;
    private readonly IMediator _mediator = mediator;

    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItemBuilder(request.OwnerId)
            .SetProject(request.ProjectName)
            .SetTitle(request.Title)
            .SetDescription(request.Description)
            .SetStatus(request.Status)
            .SetPriority(request.Priority)
            .SetDueDate(request.DueDate)
            .SetParent(request.ParentTaskId)
            .Build();

        await _context.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created task with id {TaskId}", task.TaskId);
        await _mediator.Publish(new TaskCreatedEvent(task.TaskId, request.OwnerId, task.Title), cancellationToken);
        return task.TaskId;
    }
}
