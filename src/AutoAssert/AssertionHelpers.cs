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
        var fullMessage = message + BuildReason(because, becauseArgs);

        if (AssertionScope.Current is { } scope)
        {
            scope.AddFailure(fullMessage);
            return;
        }

        throw new AssertionFailedException(fullMessage);
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

    private const string GreenAnsi = "\u001b[32m";
    private const string RedAnsi = "\u001b[31m";
    private const string ResetAnsi = "\u001b[0m";

    /// <summary>Formats an "expected" value, colorized green when <see cref="AssertionConfig.UseColorizedOutput"/> is enabled.</summary>
    public static string FormatExpected(object? value) => Colorize(Format(value), GreenAnsi);

    /// <summary>Formats an "actual"/found value, colorized red when <see cref="AssertionConfig.UseColorizedOutput"/> is enabled.</summary>
    public static string FormatActual(object? value) => Colorize(Format(value), RedAnsi);

    private static string Colorize(string text, string ansiCode) =>
        AssertionConfig.UseColorizedOutput ? $"{ansiCode}{text}{ResetAnsi}" : text;
}
