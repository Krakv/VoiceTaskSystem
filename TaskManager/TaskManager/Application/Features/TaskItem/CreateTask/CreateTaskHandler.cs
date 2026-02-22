using MediatR;
using System.Globalization;
using TaskManager.Application.Domain.Builders;
using TaskManager.Application.Domain.Entities;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;

namespace TaskManager.Application.Features.TaskItem.CreateTask;

public sealed class CreateTaskHandler(AppDbContext context, ICurrentUser user, ILogger<CreateTaskHandler> logger) : IRequestHandler<CreateTaskCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<CreateTaskHandler> _logger = logger;

    public async Task<string> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskItemBuilder(_user.UserId)
            .SetProject(request.ProjectName)
            .SetTitle(request.Title)
            .SetDescription(request.Description)
            .SetStatus(request.Status)
            .SetPriority(request.Priority)
            .SetTags(request.Tags)
            .SetDueDate(request.DueDate)
            .SetParent(request.ParentTaskId)
            .Build();

        await _context.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created task with id {TaskId}", task.TaskId);
        return task.TaskId.ToString();
    }
}
