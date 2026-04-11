using MediatR;

namespace TaskManager.Calendar.Application.Features.ExternalCalendarAccount.ExchangeOAuthCode;

public sealed record ExchangeOAuthCodeCommand(string Code, string State) : IRequest;
