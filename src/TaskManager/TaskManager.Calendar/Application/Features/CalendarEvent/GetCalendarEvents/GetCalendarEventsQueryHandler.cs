using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed class GetCalendarEventsQueryHandler(AppDbContext context) : IRequestHandler<GetCalendarEventsQuery, List<CalendarEventDto>>
{
    private readonly AppDbContext _context = context;

    public async Task<List<CalendarEventDto>> Handle(GetCalendarEventsQuery request, CancellationToken cancellationToken)
    {
        return await _context.CalendarEvent
            .Where(x => x.OwnerId == request.OwnerId)
            .Select(x => new CalendarEventDto(
                x.EventId,
                x.Title,
                x.StartTime,
                x.EndTime,
                x.Location,
                x.TaskId,
                x.ExternalAccountId
            ))
            .ToListAsync(cancellationToken);
    }
}
