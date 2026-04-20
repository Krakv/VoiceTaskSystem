using TaskManager.Notifications.Application.Services.Interfaces;

namespace TaskManager.IntegrationTests.FakeServices;

public class FakeEmailService : IEmailService
{
    public List<(string Email, string Subject, string Body)> SentEmails { get; } = new();

    public Task SendAsync(string to, string subject, string body)
    {
        SentEmails.Add((to, subject, body));
        return Task.CompletedTask;
    }
}
