using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;

public sealed record DeleteCalendarEventCommand(Guid OwnerId, Guid CalendarEventId) : IRequest;
