using System.Globalization;

namespace TaskManager.Shared.Pipeline;

public static class SimpleValidators
{
    public static bool BeValidGuid(string? value)
        => Guid.TryParse(value, out _);

    public static bool BeValidGuidOrNull(string? value)
        => string.IsNullOrWhiteSpace(value) || Guid.TryParse(value, out _);

    public static bool BeValidDateTimeOffset(string value)
        => DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, out _);
}
