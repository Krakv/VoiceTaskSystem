using MediatR;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvent;

public sealed record GetCalendarEventQuery(string OwnerId, string CalendarEventId) : IRequest<CalendarEventDto>;
