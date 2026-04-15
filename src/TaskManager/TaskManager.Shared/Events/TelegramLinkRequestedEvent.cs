namespace TaskManager.Shared.Events;

public sealed class TelegramLinkRequestedEvent(string token, long chatId) : BaseEvent
{
    public string Token { get; init; } = token;
    public long ChatId { get; init; } = chatId;

    public override string Event { get; init; } = "TelegramLinkRequested";
}
