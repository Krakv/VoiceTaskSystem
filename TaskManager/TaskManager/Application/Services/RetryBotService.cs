using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services;

public class RetryBotService(IBotService inner, ILogger<RetryBotService> logger) : IBotService
{
    private const int MaxRetries = 3;
    private readonly ILogger<RetryBotService> _logger = logger;

    public async Task SendCommand(long chatId, string command, CancellationToken ct)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await inner.SendCommand(chatId, command, ct);
                return;
            }
            catch (Exception ex) when (attempt < MaxRetries)
            {
                _logger.LogWarning("Attempt {Attempt} failed: {Error}. Retrying...", attempt, ex.Message);
                await Task.Delay(500 * attempt, ct);
            }
        }
    }
}
