using MediatR;

namespace TaskManager.Auth.Application.Features.OAuth.ExchangeOAuthCode;

public sealed record ExchangeOAuthCodeCommand(string Code, string State) : IRequest;
