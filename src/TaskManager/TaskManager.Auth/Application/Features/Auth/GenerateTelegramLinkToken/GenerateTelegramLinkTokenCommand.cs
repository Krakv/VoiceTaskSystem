using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.GenerateTelegramLinkToken;

public sealed record GenerateTelegramLinkTokenCommand : IRequest<string>;
