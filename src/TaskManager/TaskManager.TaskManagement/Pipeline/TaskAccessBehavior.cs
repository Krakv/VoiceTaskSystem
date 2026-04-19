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
        var _ = await _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TaskId == request.TaskId && x.OwnerId == _user.UserId, cancellationToken) 
            ?? throw new ValidationAppException("NOT_FOUND", "Задача не найдена");

        return await next(cancellationToken);
    }
}
