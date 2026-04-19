using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetExternalCalendars;

public sealed record GetExternalCalendarsQuery(Guid OwnerId) : IRequest<List<ExternalCalendarAccountDto>>;
