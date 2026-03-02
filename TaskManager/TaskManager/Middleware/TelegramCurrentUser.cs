namespace TaskManager.Middleware;

public sealed class TelegramCurrentUser : ICurrentUser
{
    private readonly ITelegramContextAccessor _telegramAccessor;
    public TelegramCurrentUser(ITelegramContextAccessor telegramAccessor) => _telegramAccessor = telegramAccessor;
    public Guid UserId
    {
        get
        {
            var update = _telegramAccessor.Update;

            if (update == null)
                throw new InvalidOperationException("Telegram update is null");

            // проверяем Message
            if (update.Message?.From != null)
                return Guid.Parse(update.Message.From.Id.ToString());

            // проверяем CallbackQuery
            if (update.CallbackQuery?.From != null)
                return Guid.Parse(update.CallbackQuery.From.Id.ToString());

            // проверяем InlineQuery
            if (update.InlineQuery?.From != null)
                return Guid.Parse(update.InlineQuery.From.Id.ToString());

            throw new InvalidOperationException("Не удалось определить пользователя из Update");
        }
    }
    public bool IsAuthenticated => true;
}
