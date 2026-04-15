using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.LinkTelegram;

public sealed record LinkTelegramCommand(string Token, string ChatId) : IRequest;
