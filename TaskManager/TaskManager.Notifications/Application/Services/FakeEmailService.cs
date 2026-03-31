using TaskManager.Application.Services.Interfaces;

namespace TaskManager.Application.Services;

public class FakeEmailService(ILogger<FakeEmailService> logger) : IEmailService
{
    private readonly ILogger<FakeEmailService> _logger = logger;
    public Task Send(string to, string subject, string body)
    {
        _logger.LogInformation("[FAKE EMAIL] To:{To}, Subject:{Subject}, Body:{Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
