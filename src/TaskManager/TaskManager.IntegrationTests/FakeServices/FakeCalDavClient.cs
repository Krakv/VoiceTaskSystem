using TaskManager.Calendar.Infrastructure.Interfaces;

namespace TaskManager.IntegrationTests.FakeServices;

public class FakeCalDavClient : ICalDavClient
{
    public string BuildEventUrl(string baseUrl, string uid) => $"{baseUrl}{uid}.ics";

    public Task DeleteEventAsync(string eventUrl, string token, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<string> GetUserEmailAsync(string token, CancellationToken cancellationToken)
        => Task.FromResult("testuser@yandex.ru");

    public Task UpdateEventAsync(string eventUrl, string ics, string token, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
