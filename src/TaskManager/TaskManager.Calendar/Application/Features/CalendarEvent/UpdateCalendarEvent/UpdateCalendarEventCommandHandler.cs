using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskManager.Calendar.Application.Events;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;

public sealed class UpdateCalendarEventHandler(AppDbContext context, IMediator mediator) : IRequestHandler<UpdateCalendarEventCommand>
{
    private readonly AppDbContext _context = context;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var ownerId = Guid.Parse(request.OwnerId);
        var eventId = Guid.Parse(request.CalendarEventId);

        var entity = await _context.CalendarEvent
            .FirstOrDefaultAsync(x =>
                x.EventId == eventId &&
                x.OwnerId == ownerId,
                cancellationToken);

        if (entity is null)
        {
            throw new ValidationAppException("NOT_FOUND", "Событие не найдено");
        }

        entity.Title = request.Title;
        entity.StartTime = DateTimeOffset.Parse(request.StartTime, CultureInfo.InvariantCulture);
        entity.EndTime = DateTimeOffset.Parse(request.EndTime, CultureInfo.InvariantCulture);
        entity.Location = request.Location;

        entity.TaskId = string.IsNullOrWhiteSpace(request.TaskId)
            ? null
            : Guid.Parse(request.TaskId);

        entity.ExternalAccountId = string.IsNullOrWhiteSpace(request.ExternalAccountId)
            ? null
            : Guid.Parse(request.ExternalAccountId);

        await _context.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new CalendarEventUpdatedEvent(entity.OwnerId, entity), cancellationToken);
    }
}
