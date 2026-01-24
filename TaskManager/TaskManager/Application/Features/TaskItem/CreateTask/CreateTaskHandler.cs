using MediatR;
using System.Globalization;
using TaskManager.Application.Domain.Entities;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;

namespace TaskManager.Application.Features.TaskItem.CreateTask;

public sealed class CreateTaskHandler(AppDbContext context, ICurrentUser user) : IRequestHandler<CreateTaskCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<string> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new TaskManager.Application.Domain.Entities.TaskItem()
        {
            OwnerId = _user.UserId,
            ProjectName = request.ProjectName,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            Tags = request.Tags,
            DueDate = string.IsNullOrEmpty(request.DueDate) ? null : DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture),
            ParentTaskId = string.IsNullOrEmpty(request.ParentTaskId) ? null : Guid.Parse(request.ParentTaskId)
        };

        await _context.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return task.TaskId.ToString();
    }
}
