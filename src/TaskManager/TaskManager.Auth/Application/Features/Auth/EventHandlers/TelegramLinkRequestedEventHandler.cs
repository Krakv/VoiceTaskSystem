using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Auth.Application.Features.Auth.LinkTelegram;
using TaskManager.Shared.EventHandlers;
using TaskManager.Shared.Events;

namespace TaskManager.Auth.Application.Features.Auth.EventHandlers;

public sealed class TelegramLinkRequestedEventHandler(
    IMediator mediator,
    ILogger<BaseEventHandler<TelegramLinkRequestedEvent>> logger) : BaseEventHandler<TelegramLinkRequestedEvent>(logger)
{
    private readonly IMediator _mediator = mediator;

    protected override async Task Process(
        TelegramLinkRequestedEvent notification,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new LinkTelegramCommand(notification.Token, notification.ChatId),
            cancellationToken);
    }
}