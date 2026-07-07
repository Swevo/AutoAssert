namespace AutoAssert;

/// <summary>
/// Internal helpers shared by all assertion types for building consistent failure messages
/// and applying the FluentAssertions-style "because" reasoning clause.
/// </summary>
internal static class AssertionHelpers
{
    public static string BuildReason(string because, object[] becauseArgs)
    {
        if (string.IsNullOrWhiteSpace(because))
        {
            return string.Empty;
        }

        var reason = becauseArgs is { Length: > 0 } ? string.Format(because, becauseArgs) : because;
        if (!reason.StartsWith("because", StringComparison.OrdinalIgnoreCase))
        {
            reason = "because " + reason;
        }

        return " " + reason;
    }

    public static void Fail(string message, string because, object[] becauseArgs)
    {
        throw new AssertionFailedException(message + BuildReason(because, becauseArgs));
    }

    public static string Format(object? value)
    {
        return value switch
        {
            null => "<null>",
            string s => "\"" + s + "\"",
            _ => value.ToString() ?? "<null>"
        };
    }
}
