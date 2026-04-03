using Microsoft.Extensions.Configuration;
using SpeechProcessingService.Application.Services.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace SpeechProcessingService.Infrastructure.Services;

public class DucklingClient(HttpClient httpClient, IConfiguration config) : IDateParser
{
    private readonly string _baseUrl = config["Duckling:BaseUrl"] ?? "";

    public async Task<DateTimeOffset?> ParseDateAsync(string text, TimeZoneInfo userTimeZone, string locale = "ru_RU")
    {
        var userNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
        var reftime = new DateTimeOffset(userNow, userTimeZone.GetUtcOffset(userNow))
            .ToUnixTimeMilliseconds();

        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("locale", locale),
            new KeyValuePair<string, string>("text", text),
            new KeyValuePair<string, string>("dims", "[\"time\"]"),
            new KeyValuePair<string, string>("reftime", reftime.ToString()),
        ]);

        var response = await httpClient.PostAsync($"{_baseUrl}/parse", content);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<JsonElement[]>(json);

        if (results is null || results.Length == 0) return null;

        var valueStr = results[0]
            .GetProperty("value")
            .GetProperty("value")
            .GetString();

        return DateTimeOffset.TryParse(
            valueStr,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var dto)
            ? dto.UtcDateTime
            : null;
    }
}
