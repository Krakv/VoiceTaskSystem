using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.TaskManagement.Pipeline;

public class TaskAccessBehavior<TRequest, TResponse>(AppDbContext context, ICurrentUser user) : IPipelineBehavior<TRequest, TResponse> where TRequest : ITaskAccessRequest
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TaskId)) return await next(cancellationToken);

        var task = await _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TaskId == Guid.Parse(request.TaskId), cancellationToken);

        if (task == null)
            throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        if (task.OwnerId != _user.UserId)
            throw new ValidationAppException("FORBIDDEN", "Нет доступа к задаче");

        return await next(cancellationToken);
    }
}
