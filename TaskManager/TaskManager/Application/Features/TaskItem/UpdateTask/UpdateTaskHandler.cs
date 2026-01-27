using MediatR;
using System.Globalization;
using TaskManager.Application.Features.TaskItem.DeleteTask;
using TaskManager.Exceptions;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;

namespace TaskManager.Application.Features.TaskItem.UpdateTask;

public sealed class UpdateTaskHandler(AppDbContext context, ICurrentUser user) : IRequestHandler<UpdateTaskCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

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
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = string.IsNullOrEmpty(request.DueDate) ? null : DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture);
        task.Tags = request.Tags;
        task.ParentTaskId = string.IsNullOrEmpty(request.ParentTaskId) ? null : Guid.Parse(request.ParentTaskId);
        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return task.TaskId.ToString();
    }
}
