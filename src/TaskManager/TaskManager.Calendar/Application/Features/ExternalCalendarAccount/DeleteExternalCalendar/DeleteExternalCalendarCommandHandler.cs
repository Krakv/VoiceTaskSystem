using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;

public sealed class DeleteExternalCalendarCommandHandler(AppDbContext context, ICurrentUser currentUser) : IRequestHandler<DeleteExternalCalendarCommand>
{
    public async Task Handle(DeleteExternalCalendarCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var entity = await context.ExternalCalendarAccount
            .FirstOrDefaultAsync(x =>
                x.ExternalCalendarAccountId == request.Id &&
                x.OwnerId == userId,
                cancellationToken);

        if (entity is null)
            return;

        context.ExternalCalendarAccount.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}
