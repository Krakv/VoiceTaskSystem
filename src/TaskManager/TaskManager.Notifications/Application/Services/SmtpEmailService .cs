using Microsoft.Extensions.Options;
using System.Net.Mail;
using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.Notifications.Application.Services;

public class SmtpEmailService : IEmailService
{
    public Task Send(string to, string subject, string body)
    {
        // TODO: реализовать при наличии SMTP конфигурации
        throw new NotImplementedException();
    }
}
