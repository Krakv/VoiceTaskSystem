using MediatR;
using TaskManager.Calendar.Application.Events;
using TaskManager.Repository.Context;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;

public sealed class CreateCalendarEventCommandHandler(AppDbContext dbContext, IMediator mediator) : IRequestHandler<CreateCalendarEventCommand, Guid>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IMediator _mediator = mediator;

    public async Task<Guid> Handle(CreateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var entity = new TaskManager.Shared.Domain.Entities.CalendarEvent
        {
            EventId = Guid.NewGuid(),
            Title = request.Title,
            OwnerId = request.OwnerId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Location = request.Location,
            TaskId = request.TaskId,
            ExternalAccountId = request.ExternalAccountId
        };

        _dbContext.CalendarEvent.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new CalendarEventCreatedEvent(entity.OwnerId, entity), cancellationToken);

        return entity.EventId;
    }
}
