namespace TaskManager.ApiGateway.Middleware;

public interface ITelegramContextAccessor
{
    Telegram.Bot.Types.Update? Update { get; set; }
}
