namespace TaskManager.Notifications.Application.Services.Interfaces;

public interface IBotService
{
    Task SendCommand(long chatId, string command, CancellationToken stoppingToken);   
}
