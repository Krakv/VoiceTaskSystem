using MediatR;
using TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvents;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.GetCalendarEvent;

public sealed record GetCalendarEventQuery(Guid OwnerId, Guid CalendarEventId) : IRequest<CalendarEventDto>;
