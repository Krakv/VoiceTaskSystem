namespace TaskManager.Calendar.Infrastructure.Interfaces;

public interface ICalDavClient
{
    Task<string> GetUserEmailAsync(string token, CancellationToken cancellationToken);
    string BuildEventUrl(string baseUrl, string uid);
    Task UpdateEventAsync(string eventUrl, string ics, string token, CancellationToken cancellationToken);
    Task DeleteEventAsync(string eventUrl, string token, CancellationToken cancellationToken);
}
