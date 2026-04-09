using MediatR;
using TaskManager.Shared.Domain.Builders;
using TaskManager.Repository.Context;
using TaskManager.Shared.Events;
using TaskManager.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.CreateTask;

public sealed class CreateTaskHandler(AppDbContext context, ICurrentUser user, ILogger<CreateTaskHandler> logger, IMediator mediator) : IRequestHandler<CreateTaskCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<CreateTaskHandler> _logger = logger;
    private readonly IMediator _mediator = mediator;

    public async Task<string> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItemBuilder(_user.UserId)
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
        await _mediator.Publish(new TaskCreatedEvent(task.TaskId, _user.UserId, task.Title), cancellationToken);
        return task.TaskId.ToString();
    }
}
