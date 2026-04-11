using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using TaskManager.Calendar.Infrastructure.Interfaces;

namespace TaskManager.Calendar.Infrastructure;

public sealed class CalDavClient(HttpClient httpClient) : ICalDavClient
{
    private const string CalDavBaseUrl = "https://caldav.yandex.ru/";

    public async Task<string> GetUserEmailAsync(string token, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            new HttpMethod("PROPFIND"),
            CalDavBaseUrl);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("OAuth", token);
        request.Headers.Add("Depth", "0");

        request.Content = new StringContent("""
            <?xml version="1.0" encoding="utf-8"?>
            <propfind xmlns="DAV:">
              <prop>
                <current-user-principal/>
              </prop>
            </propfind>
            """,
            Encoding.UTF8,
            "application/xml");

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var xml = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = XDocument.Parse(xml);
        XNamespace dav = "DAV:";

        var href = doc
            .Descendants(dav + "current-user-principal")
            .Elements(dav + "href")
            .FirstOrDefault()?.Value
            ?? throw new Exception($"Не удалось найти principal. XML: {xml}");

        return href
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Last();
    }

    public string BuildEventUrl(string baseUrl, string uid) => $"{baseUrl}{uid}.ics";

    public async Task UpdateEventAsync(string eventUrl, string ics, string token, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, eventUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", token);
        request.Content = new StringContent(ics);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("text/ics");

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteEventAsync(string eventUrl, string token, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, eventUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", token);

        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}