using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Notifications.Pipeline;

public class NotificationAccessBehavior<TRequest, TResponse>(AppDbContext context, ICurrentUser user) : IPipelineBehavior<TRequest, TResponse> where TRequest : INotificationAccessRequest
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _user = user;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NotificationId)) return await next(cancellationToken);

        var notif = await _context.NotificationItem
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.NotificationId == Guid.Parse(request.NotificationId), cancellationToken);

        if (notif == null)
            throw new ValidationAppException("NOT_FOUND", "Уведомление не найдено");

        if (notif.OwnerId != _user.UserId)
            throw new ValidationAppException("FORBIDDEN", "Нет доступа к уведомлению");

        return await next(cancellationToken);
    }
}
