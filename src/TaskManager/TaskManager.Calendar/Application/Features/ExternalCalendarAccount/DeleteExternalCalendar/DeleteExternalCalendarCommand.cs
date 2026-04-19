using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.DeleteExternalCalendar;

public sealed record DeleteExternalCalendarCommand(Guid OwnerId, Guid Id) : IRequest;
