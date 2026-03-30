using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services.Factories;

public class FakeEmailServiceFactory(ILogger<FakeEmailService> logger) : EmailServiceFactory
{
    private readonly ILogger<FakeEmailService> _logger = logger;

    public override IEmailService CreateEmailService()
        => new FakeEmailService(_logger);
}
