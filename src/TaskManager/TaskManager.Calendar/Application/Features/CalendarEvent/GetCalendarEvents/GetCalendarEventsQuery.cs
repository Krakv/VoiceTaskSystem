using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed record GetCalendarEventsQuery(string OwnerId) : IRequest<List<CalendarEventDto>>;
