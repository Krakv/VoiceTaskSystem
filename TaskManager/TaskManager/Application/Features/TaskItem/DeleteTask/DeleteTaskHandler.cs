using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Features.TaskItem.CreateTask;
using TaskManager.Exceptions;
using TaskManager.Infrastructure.Repository;
using TaskManager.Middleware;

namespace TaskManager.Application.Features.TaskItem.DeleteTask;

public sealed class DeleteTaskHandler(AppDbContext context, ICurrentUser user, ILogger<DeleteTaskHandler> logger) : IRequestHandler<DeleteTaskCommand, string>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;
    private readonly ILogger<DeleteTaskHandler> _logger = logger;

    public async Task<string> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.TaskItems.FindAsync([Guid.Parse(request.TaskId)], cancellationToken: cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (_user.UserId != task.OwnerId)
        {
            throw new ValidationAppException("FORBIDDEN", "Нет доступа");
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted task with id {TaskId}", task.TaskId);
        return request.TaskId;
    }
}
