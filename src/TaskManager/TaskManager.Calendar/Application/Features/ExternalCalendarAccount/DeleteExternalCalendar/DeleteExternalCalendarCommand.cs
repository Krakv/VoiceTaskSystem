using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;

public sealed record DeleteExternalCalendarCommand(string Id) : IRequest;
