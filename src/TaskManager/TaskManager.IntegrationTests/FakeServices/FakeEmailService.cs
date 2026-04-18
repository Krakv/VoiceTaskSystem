using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.IntegrationTests.FakeServices;

public class FakeEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }
}
