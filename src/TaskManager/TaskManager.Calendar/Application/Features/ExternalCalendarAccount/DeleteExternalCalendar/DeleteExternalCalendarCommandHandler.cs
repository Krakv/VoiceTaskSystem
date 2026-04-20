using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;

public sealed class DeleteExternalCalendarCommandHandler(AppDbContext context) : IRequestHandler<DeleteExternalCalendarCommand>
{
    public async Task Handle(DeleteExternalCalendarCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.ExternalCalendarAccount
            .FirstOrDefaultAsync(x =>
                x.ExternalCalendarAccountId == request.Id &&
                x.OwnerId == request.OwnerId,
                cancellationToken)
            ?? throw new ValidationAppException("NOT_FOUND", "Внешний календарь не найден");

        context.ExternalCalendarAccount.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}
