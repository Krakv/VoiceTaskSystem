using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed record GetCalendarEventsQuery(Guid OwnerId) : IRequest<List<CalendarEventDto>>;
