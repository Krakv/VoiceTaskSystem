using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetAuthorizeUrl;

public sealed record GetAuthorizeUrlQuery(Guid OwnerId) : IRequest<string>;
