using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Notifications.Application.Services.Interfaces;
using TaskManager.Notifications.Config;

namespace TaskManager.Notifications.Application.Services.Factories;

public class SmtpEmailServiceFactory(IOptions<SmtpOptions> options, ILogger<SmtpEmailService> logger) : EmailServiceFactory
{
    public override IEmailService CreateEmailService()
        => new SmtpEmailService(options, logger);
}
