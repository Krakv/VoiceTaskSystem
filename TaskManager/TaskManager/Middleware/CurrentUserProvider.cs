namespace TaskManager.Middleware;

public sealed class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor,
    ITelegramContextAccessor telegramAccessor
    ) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ITelegramContextAccessor _telegramAccessor = telegramAccessor;

    public ICurrentUser GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            return new HttpCurrentUser(_httpContextAccessor);

        if (_telegramAccessor.Update != null)
            return new TelegramCurrentUser(_telegramAccessor);

        throw new InvalidOperationException("Не удалось определить источник пользователя");
    }
}

