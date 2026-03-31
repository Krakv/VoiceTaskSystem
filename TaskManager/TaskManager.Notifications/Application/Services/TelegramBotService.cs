using Microsoft.Extensions.Options;
using TaskManager.Config;
using TaskManager.Notifications.Application.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using static Telegram.Bot.TelegramBotClient;

namespace TaskManager.Notifications.Application.Services;

public class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBotService _adapter;
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(IServiceScopeFactory scopeFactory, IBotService adapter, ITelegramBotClient botClient, ILogger<TelegramBotService> logger)
    {
        _botClient = botClient;
        _adapter = adapter;
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            [
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            ],
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            DropPendingUpdates = true,
        };
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _logger.LogInformation("Started TelegramBotService");
        await _botClient.ReceiveAsync(UpdateHandler, ErrorHandler, _receiverOptions, stoppingToken); // Запускаем бота
    }

    private async Task UpdateHandler(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
    {
        try
        {
            // Тут обрабатываем приходящие Update`ы
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text) // Если пришло текстовое сообщение
            {
                var chatId = update.Message.Chat.Id; // Получаем Id чата, из которого пришло сообщение
                var messageText = update.Message.Text; // Получаем текст сообщения
                _logger.LogInformation("Received a message from chat {ChatId}: {MessageText}", chatId, messageText);

                using var scope = _scopeFactory.CreateScope();
                
                // Пример ответа на сообщение
                await _adapter.SendCommand(chatId, "Message Recieved", stoppingToken: cancellationToken);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateHandler");
        }
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Тут обрабатываем ошибки, связанные с Bot API
        _logger.LogError(exception, "Error in UpdateHandler");
        return Task.CompletedTask;
    }
}
