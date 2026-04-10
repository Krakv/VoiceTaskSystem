using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TaskManager.Auth.Config;
using TaskManager.Auth.DTOs;

namespace TaskManager.Auth.Infrastructure;

public sealed class YandexOAuthClient(HttpClient httpClient, IOptions<YandexOAuthConfig> options)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly YandexOAuthConfig _options = options.Value;

    public async Task<OAuthTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            _options.TokenUrl);

        var creds = $"{_options.ClientId}:{_options.ClientSecret}";
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(creds));

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", base64);

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        });

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OAuthTokenResponse>()
                     ?? throw new Exception("Empty OAuth response");

        return result;
    }

    public async Task<OAuthTokenResponse> ExchangeCodeAsync(string code)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret
        });

        var response = await _httpClient.PostAsync(_options.TokenUrl, content);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OAuthTokenResponse>()
                     ?? throw new Exception("Empty OAuth response");

        return result;
    }

    public Task<string> GetAuthorizeUrl(string state)
    {
        var url =
            _options.AuthorizeUri +
            "?response_type=code" +
            $"&client_id={_options.ClientId}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            $"&state={Uri.EscapeDataString(state)}";

        return Task.FromResult(url);
    }
}
