using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services.Factories;

public class FakeEmailServiceFactory : EmailServiceFactory
{
    private readonly ILogger<FakeEmailService> _logger;

    public FakeEmailServiceFactory(ILogger<FakeEmailService> logger)
    {
        _logger = logger;
    }

    public override IEmailService CreateEmailService()
        => new FakeEmailService(_logger);
}
