using TaskManager.Notifications.Application.Services.Interfaces;
using Telegram.Bot;

namespace TaskManager.Notifications.Application.Services;

public class TelegramBotAdapter(ITelegramBotClient botClient, ILogger<TelegramBotAdapter> logger) : IBotService
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<TelegramBotAdapter> _logger = logger;

    public async Task SendCommand(long chatId, string command, CancellationToken stoppingToken)
    {
        _logger.LogInformation("Send message to chat {ChatId}", chatId);
        await _botClient.SendMessage(chatId, command, cancellationToken: stoppingToken);
    }
}
