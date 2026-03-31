namespace TaskManager.Notifications.Application.Services.Interfaces;

public interface IEmailService
{
    Task Send(string to, string subject, string body);
}
