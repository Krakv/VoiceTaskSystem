using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Shared.Events;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

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
        _logger.LogInformation("TelegramBotService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting Telegram polling...");

                await _botClient.ReceiveAsync(
                    UpdateHandler,
                    ErrorHandler,
                    _receiverOptions,
                    stoppingToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("Telegram bot crashed");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("TelegramBotService stopped");
    }

    private async Task UpdateHandler(
    ITelegramBotClient botClient,
    Telegram.Bot.Types.Update update,
    CancellationToken cancellationToken)
    {
        try
        {
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                _logger.LogInformation(
                    "Received a message from chat {ChatId}: {MessageText}",
                    chatId,
                    messageText);

                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                if (messageText.StartsWith("/start"))
                {
                    var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 1)
                    {
                        var token = parts[1];

                        await mediator.Publish(new TelegramLinkRequestedEvent(token, chatId), cancellationToken);

                        await _adapter.SendCommand(
                            chatId,
                            "Telegram успешно привязан ✅",
                            stoppingToken: cancellationToken);

                        return;
                    }
                }

                // обычная логика
                await _adapter.SendCommand(
                    chatId,
                    "Message received",
                    stoppingToken: cancellationToken);
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
