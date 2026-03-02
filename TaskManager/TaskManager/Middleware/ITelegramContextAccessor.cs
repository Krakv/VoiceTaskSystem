namespace TaskManager.Middleware;

public interface ITelegramContextAccessor
{
    Telegram.Bot.Types.Update? Update { get; set; }
}
