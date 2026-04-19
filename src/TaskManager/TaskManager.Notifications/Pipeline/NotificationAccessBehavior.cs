using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Pipeline;

public class NotificationAccessBehavior<TRequest, TResponse>(AppDbContext context) : IPipelineBehavior<TRequest, TResponse> where TRequest : INotificationAccessRequest
{
    private readonly AppDbContext _context = context;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {

        var _ = await _context.NotificationItem
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.NotificationId == request.NotificationId && x.OwnerId == request.OwnerId, cancellationToken) 
            ?? throw new ValidationAppException("NOT_FOUND", "Уведомление не найдено");

        return await next(cancellationToken);
    }
}
