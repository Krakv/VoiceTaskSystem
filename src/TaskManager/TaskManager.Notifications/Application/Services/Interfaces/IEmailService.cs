namespace TaskManager.Notifications.Application.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}
