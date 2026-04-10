using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

public sealed record GetCalendarEventsQuery : IRequest<List<CalendarEventDto>>;
