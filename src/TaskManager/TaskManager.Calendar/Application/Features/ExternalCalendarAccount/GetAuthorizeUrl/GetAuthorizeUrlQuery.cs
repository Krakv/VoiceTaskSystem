using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.GetAuthorizeUrl;

public sealed record GetAuthorizeUrlQuery() : IRequest<string>;
