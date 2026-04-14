using MediatR;

namespace TaskManager.Calendar.Application.Features.CalendarEvent.DeleteCalendarEvent;

public sealed record DeleteCalendarEventCommand(string OwnerId, string CalendarEventId) : IRequest;
