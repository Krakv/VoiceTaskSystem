using Microsoft.Extensions.Options;
using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.Notifications.Application.Services.Factories;

public class SmtpEmailServiceFactory : EmailServiceFactory
{
    public override IEmailService CreateEmailService()
        => new SmtpEmailService();
}
