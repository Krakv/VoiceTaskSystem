using SpeechProcessingService.Application.Models;

namespace SpeechProcessingService.Application.Services.Interfaces;

public interface IEntityNormalizer
{
    Task<TaskItem> NormalizeAsync(Dictionary<string, string> entities, TimeZoneInfo userTimeZone);
}
