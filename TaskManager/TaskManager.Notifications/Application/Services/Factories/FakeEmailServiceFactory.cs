using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.Notifications.Application.Services.Factories;

public class FakeEmailServiceFactory(ILogger<FakeEmailService> logger) : EmailServiceFactory
{
    private readonly ILogger<FakeEmailService> _logger = logger;

    public override IEmailService CreateEmailService()
        => new FakeEmailService(_logger);
}
