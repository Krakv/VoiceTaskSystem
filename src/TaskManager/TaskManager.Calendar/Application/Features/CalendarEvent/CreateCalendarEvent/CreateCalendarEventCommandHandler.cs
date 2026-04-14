using MediatR;
using System.Globalization;
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
            OwnerId = Guid.Parse(request.OwnerId),
            StartTime = DateTimeOffset.Parse(request.StartTime, CultureInfo.InvariantCulture),
            EndTime = DateTimeOffset.Parse(request.EndTime, CultureInfo.InvariantCulture),
            Location = request.Location,
            TaskId = string.IsNullOrWhiteSpace(request.TaskId)
                ? null
                : Guid.Parse(request.TaskId),
            ExternalAccountId = string.IsNullOrWhiteSpace(request.ExternalAccountId)
                ? null
                : Guid.Parse(request.ExternalAccountId)
        };

        _dbContext.CalendarEvent.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new CalendarEventCreatedEvent(entity.OwnerId, entity), cancellationToken);

        return entity.EventId;
    }
}
