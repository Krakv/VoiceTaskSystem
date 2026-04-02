namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IDateParser
{
    Task<DateTimeOffset?> ParseDateAsync(string text, TimeZoneInfo userTimeZone, string locale = "ru_RU");
}
