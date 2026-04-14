using System.Text;
using TaskManager.Calendar.Application.Interfaces;
using TaskManager.Shared.Domain.Entities;

namespace TaskManager.Calendar.Application.Services;

public sealed class CalendarIcsGenerator : ICalendarIcsGenerator
{
    public string Generate(CalendarEvent e)
    {
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//TaskManager//EN");

        sb.AppendLine("BEGIN:VEVENT");

        var uid = e.EventId.ToString();
        sb.AppendLine($"UID:{uid}");

        sb.AppendLine($"DTSTAMP:{Format(e.CreatedAt)}");

        sb.AppendLine($"DTSTART:{Format(e.StartTime)}");
        sb.AppendLine($"DTEND:{Format(e.EndTime)}");

        sb.AppendLine($"SUMMARY:{Escape(e.Title ?? "Новое событие")}");

        if (!string.IsNullOrWhiteSpace(e.Location))
            sb.AppendLine($"LOCATION:{Escape(e.Location)}");

        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        return sb.ToString();
    }

    private static string Format(DateTimeOffset dt)
        => dt.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'");

    private static string Escape(string value)
        => value
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\n", "\\n");
}
