using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;

public sealed record DeleteExternalCalendarCommand(Guid Id) : IRequest;
