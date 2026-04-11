using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Repository.Context;
using TaskManager.Shared.Interfaces;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed class GetCalendarEventsQueryHandler(AppDbContext context, ICurrentUser currentUser) : IRequestHandler<GetCalendarEventsQuery, List<CalendarEventDto>>
{
    private readonly AppDbContext _context = context;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<List<CalendarEventDto>> Handle(GetCalendarEventsQuery request, CancellationToken cancellationToken)
    {
        return await _context.CalendarEvent
            .Where(x => x.OwnerId == _currentUser.UserId)
            .Select(x => new CalendarEventDto(
                x.EventId,
                x.StartTime.ToString(),
                x.EndTime.ToString(),
                x.Location,
                x.TaskId
            ))
            .ToListAsync(cancellationToken);
    }
}
