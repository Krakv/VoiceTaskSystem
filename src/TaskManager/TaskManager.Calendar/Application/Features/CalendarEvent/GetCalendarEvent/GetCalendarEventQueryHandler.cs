using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;
using TaskManager.Repository.Context;
using TaskManager.Shared.Exceptions;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvent;

public sealed class GetCalendarEventQueryHandler(AppDbContext dbContext) : IRequestHandler<GetCalendarEventQuery, CalendarEventDto>
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task<CalendarEventDto> Handle(GetCalendarEventQuery request, CancellationToken cancellationToken)
    {
        var calendarEvent =  await _dbContext.CalendarEvent
            .Where(x => x.OwnerId == request.OwnerId &&
                        x.EventId ==request.CalendarEventId
                        )
            .Select(x => new CalendarEventDto(
                x.EventId,
                x.Title,
                x.StartTime,
                x.EndTime,
                x.Location,
                x.TaskId,
                x.ExternalAccountId
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return calendarEvent ?? throw new ValidationAppException("NOT_FOUND", "Событие не найдено");
    }
}
