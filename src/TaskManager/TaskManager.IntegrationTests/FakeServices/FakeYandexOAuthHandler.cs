using System.Net;
using System.Text;

namespace TaskManager.IntegrationTests.FakeServices;

public class FakeYandexOAuthHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = """
        {
            "access_token": "fake_access_token",
            "refresh_token": "fake_refresh_token",
            "expires_in": 3600
        }
        """;

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}
