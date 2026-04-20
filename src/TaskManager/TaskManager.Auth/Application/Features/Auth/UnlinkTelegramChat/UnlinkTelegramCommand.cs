using MediatR;

namespace TaskManager.Auth.Application.Features.Auth.UnlinkTelegramChat;

public sealed record UnlinkTelegramCommand(Guid OwnerId) : IRequest;
