using MediatR;
using TaskManager.Calendar.Application.Events;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.CreateCalendarEvent;

public sealed class CreateCalendarEventCommandHandler(AppDbContext dbContext, ICurrentUser currentUser, IMediator mediator) : IRequestHandler<CreateCalendarEventCommand, Guid>
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;

    public async Task<Guid> Handle(CreateCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var entity = new TaskManager.Shared.Domain.Entities.CalendarEvent
        {
            EventId = Guid.NewGuid(),
            OwnerId = _currentUser.UserId,
            StartTime = DateTimeOffset.Parse(request.StartTime),
            EndTime = DateTimeOffset.Parse(request.EndTime),
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
