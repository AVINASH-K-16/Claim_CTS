namespace TaskManagement.Shared.Extensions;

/// <summary>
/// String extension methods
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Check if string is null, empty, or whitespace
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Truncate string to maximum length
    /// </summary>
    public static string Truncate(this string value, int maxLength)
    {
        if (value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength) + "...";
    }
}
