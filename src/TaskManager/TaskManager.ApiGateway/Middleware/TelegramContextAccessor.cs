namespace TaskManager.ApiGateway.Middleware;

public class TelegramContextAccessor : ITelegramContextAccessor
{
    public Telegram.Bot.Types.Update? Update { get; set; }
}
