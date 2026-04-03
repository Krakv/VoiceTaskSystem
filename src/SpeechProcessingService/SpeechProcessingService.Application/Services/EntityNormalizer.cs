using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using SpeechProcessingService.Application.Models;
using SpeechProcessingService.Application.Services.Interfaces;

namespace SpeechProcessingService.Infrastructure.Services;

public class EntityNormalizer(IServiceScopeFactory scopeFactory) : IEntityNormalizer
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task<TaskItem> NormalizeAsync(Dictionary<string, string> entities, TimeZoneInfo userTimeZone)
    {
        var task = new TaskItem
        {
            Title = NormalizeText(GetValue(entities, "TITLE")) ?? "Без названия",
            Description = NormalizeText(GetValue(entities, "DESCRIPTION")),
            ProjectName = NormalizeText(GetValue(entities, "PROJECT")),
            ParentTaskName = NormalizeText(GetValue(entities, "PARENT_TASK")),

            Priority = MapPriority(GetValue(entities, "PRIORITY")),
            Status = MapStatus(GetValue(entities, "STATUS")),
            DueDate = await ParseDate(GetValue(entities, "DUE_DATE"), userTimeZone)
        };

        return task;
    }

    private static string? GetValue(Dictionary<string, string> entities, string key)
    {
        return entities.TryGetValue(key, out var value)
            ? value
            : null;
    }

    private static string? NormalizeText(string? input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? null
            : input.Trim();
    }


    private static TaskItemPriority MapPriority(string? value)
    {
        var v = Normalize(value);

        return v switch
        {
            "высокий" or "высокая" or "high" => TaskItemPriority.High,
            "средний" or "средняя" or "medium" => TaskItemPriority.Medium,
            "низкий" or "низкая" or "low" => TaskItemPriority.Low,
            _ => TaskItemPriority.Low
        };
    }


    private static TaskItemStatus MapStatus(string? value)
    {
        var v = Normalize(value);

        return v switch
        {
            "новую" or "новая" or "новый" or "new" => TaskItemStatus.New,
            "в процессе" or "делается" => TaskItemStatus.InProgress,
            "сделано" or "выполнено" or "готово" => TaskItemStatus.Done,
            "отменено" => TaskItemStatus.Canceled,
            _ => TaskItemStatus.New
        };
    }


    private async Task<DateTimeOffset?> ParseDate(string? input, TimeZoneInfo userTimeZone)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        input = input.Trim();

        var culture = new CultureInfo("ru-RU");

        // Пробуем стандартный парсинг
        if (DateTime.TryParse(input, culture, DateTimeStyles.AssumeLocal, out var dt))
        {
            return new DateTimeOffset(dt);
        }

        // fallback: "27 декабря"
        using var scope = _scopeFactory.CreateScope();
        var dateParser = scope.ServiceProvider.GetRequiredService<IDateParser>();
        var dueDate = await dateParser.ParseDateAsync(input, userTimeZone);

        return dueDate;
    }

    private static string Normalize(string? input)
    {
        return input?
            .Trim()
            .ToLowerInvariant() ?? string.Empty;
    }
}