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
            .Where(x => x.OwnerId == Guid.Parse(request.OwnerId) &&
                        x.EventId == Guid.Parse(request.CalendarEventId)
                        )
            .Select(x => new CalendarEventDto(
                x.EventId,
                x.Title,
                x.StartTime.ToString(),
                x.EndTime.ToString(),
                x.Location,
                x.TaskId,
                x.ExternalAccountId
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (calendarEvent == null)
        {
            throw new ValidationAppException("NOT_FOUND", "Событие не найдено");
        }

        return calendarEvent;
    }
}
