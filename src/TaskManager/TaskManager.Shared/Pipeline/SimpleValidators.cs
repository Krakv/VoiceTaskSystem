using System.Globalization;
using System.Text.Json;

namespace TaskManager.Shared.Pipeline;

public static class SimpleValidators
{
    public static bool BeValidGuid(string? value)
        => Guid.TryParse(value, out _);

    public static bool BeValidGuidOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) || Guid.TryParse(value, out _);

    public static bool BeValidDateTimeOffset(string? value)
        => !string.IsNullOrWhiteSpace(value) && DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, out _);

    public static bool BeValidDateTimeOffsetOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) || DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, out _);

    public static bool BeValidEnumOrNull<TEnum>(string? value)
        where TEnum : struct, Enum
        => string.IsNullOrWhiteSpace(value) || Enum.TryParse<TEnum>(value, true, out _);

    public static bool BeValidIntPositive(string? value)
        => int.TryParse(value, out var v) && v > 0;

    public static bool BeValidIntNonNegative(string? value)
        => int.TryParse(value, out var v) && v >= 0;

    public static bool BeValidEnum<TEnum>(string value)
        where TEnum : struct, Enum
        => Enum.TryParse<TEnum>(value, true, out _);

    public static bool BeValidJsonOrNull(object? obj)
    {
        if (obj is null) return true;
        try
        {
            JsonDocument.Parse(obj.ToString()!);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool BeValidJson(object obj)
    {
        try
        {
            JsonDocument.Parse(obj.ToString()!);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
