using MediatR;
using System.Globalization;
using TaskManager.Exceptions;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;

namespace TaskManager.Application.Features.TaskItem.UpdateTaskPatch;

public sealed class UpdateTaskPatchHandler(AppDbContext context, ICurrentUser user) : IRequestHandler<UpdateTaskPatchCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<string> Handle(UpdateTaskPatchCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([Guid.Parse(request.TaskId)], cancellationToken)
        ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (_user.UserId != task.OwnerId)
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");

        if (request.ProjectName is not null) task.ProjectName = request.ProjectName;
        if (request.Title is not null) task.Title = request.Title;
        if (request.Description is not null) task.Description = request.Description;
        if (request.Status is not null) task.Status = request.Status;
        if (request.Priority is not null) task.Priority = request.Priority;
        if (request.DueDate is not null) task.DueDate = string.IsNullOrEmpty(request.DueDate) ? null : DateTimeOffset.Parse(request.DueDate, CultureInfo.InvariantCulture);
        if (request.Tags is not null) task.Tags = request.Tags;
        if (request.ParentTaskId is not null)
            task.ParentTaskId = string.IsNullOrEmpty(request.ParentTaskId) ? null : Guid.Parse(request.ParentTaskId);

        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return task.TaskId.ToString();
    }
}
