using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Calendar.Application.Events;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;

public sealed class DeleteCalendarEventHandler(AppDbContext context, IMediator mediator) : IRequestHandler<DeleteCalendarEventCommand>
{
    private readonly AppDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(DeleteCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.Parse(request.CalendarEventId);
        var ownerid = Guid.Parse(request.OwnerId);

        var entity = await _context.CalendarEvent
            .FirstOrDefaultAsync(x =>
                x.EventId == id &&
                x.OwnerId == ownerid,
                cancellationToken);

        if (entity is null)
        {
            throw new ValidationAppException("NOT_FOUND", "Событие не найдено");
        }

        _context.CalendarEvent.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new CalendarEventDeletedEvent(entity.OwnerId, entity.EventId, entity.ExternalAccountId), cancellationToken);
    }
}
