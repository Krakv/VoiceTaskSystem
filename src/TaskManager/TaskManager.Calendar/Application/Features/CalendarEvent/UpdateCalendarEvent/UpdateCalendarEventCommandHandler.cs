using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Calendar.Application.Events;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.UpdateCalendarEvent;

public sealed class UpdateCalendarEventHandler(AppDbContext context, ICurrentUser currentUser, IMediator mediator) : IRequestHandler<UpdateCalendarEventCommand>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;
    public async Task Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CalendarEvent
            .FirstOrDefaultAsync(x =>
                x.EventId == Guid.Parse(request.Id) &&
                x.OwnerId == _currentUser.UserId,
                cancellationToken);

        if (entity is null)
        {
            throw new ValidationAppException("NOT_FOUND", "Событие не найдено");
        }

        entity.StartTime = DateTimeOffset.Parse(request.StartTime);
        entity.EndTime = DateTimeOffset.Parse(request.EndTime);
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
