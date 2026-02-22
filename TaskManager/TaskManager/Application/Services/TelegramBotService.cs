using Microsoft.Extensions.Options;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Config;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using static Telegram.Bot.TelegramBotClient;

namespace TaskManager.Application.Services;

public class TelegramBotService : BackgroundService, IBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions;
    private readonly IServiceScopeFactory _scopeFactory;

    public TelegramBotService(IServiceScopeFactory scopeFactory, IOptions<TelegramBotConfig> config)
    {
        _botClient = new TelegramBotClient(config.Value.AuthToken); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
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
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
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
                Console.WriteLine($"Received a message from chat {chatId}: {messageText}");

                using var scope = _scopeFactory.CreateScope();
                
                // Пример ответа на сообщение
                await SendCommand(chatId, "Message Recieved", stoppingToken: cancellationToken);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateHandler: {ex.Message}");
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Тут обрабатываем ошибки, связанные с Bot API
        Console.WriteLine($"An error occurred: {exception.Message}");
        return Task.CompletedTask;
    }

    public async Task SendCommand(long chatId, string command, CancellationToken stoppingToken)
    {
        await _botClient.SendMessage(chatId, command, cancellationToken: stoppingToken);
    }
}
