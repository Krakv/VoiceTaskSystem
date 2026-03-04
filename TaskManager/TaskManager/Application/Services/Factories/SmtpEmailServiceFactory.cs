using Microsoft.Extensions.Options;
using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services.Factories;

public class SmtpEmailServiceFactory : EmailServiceFactory
{
    public override IEmailService CreateEmailService()
        => new SmtpEmailService();
}
